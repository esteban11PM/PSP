using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/assessment-types")]
public class AssessmentTypesController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly IAssessmentService _svc;

    public AssessmentTypesController(SchoolDbContext db, IAssessmentService svc)
    {
        _db = db; _svc = svc;
    }

    // LISTAR rubros de una oferta
    [HttpGet("by-offering/{offeringId:int}")]
    public async Task<IActionResult> ByOffering(int offeringId, CancellationToken ct = default)
    {
        var items = await _db.AssessmentTypes.AsNoTracking()
            .Where(a => a.SubjectOfferingId == offeringId)
            .Select(a => new AssessmentTypeResponseDTO(a.Id, a.SubjectOfferingId, a.Name, a.Weight))
            .ToListAsync(ct);
        return Ok(items);
    }

    // OBTENER por id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssessmentTypeResponseDTO>> GetById(int id, CancellationToken ct = default)
    {
        var a = await _db.AssessmentTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a is null) return NotFound();
        return Ok(new AssessmentTypeResponseDTO(a.Id, a.SubjectOfferingId, a.Name, a.Weight));
    }

    // CREAR rubro (usa service para validar pesos=100 después de crear)
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] AssessmentTypeRequestDTO dto)
        => Ok(await _svc.CreateAsync(dto));

    // ACTUALIZAR rubro (nombre/peso)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AssessmentTypeRequestDTO dto, CancellationToken ct = default)
    {
        var a = await _db.AssessmentTypes
            .Include(x => x.SubjectOffering).ThenInclude(o => o.Period)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a is null) return NotFound();

        if (a.SubjectOffering.IsClosed || a.SubjectOffering.Period.Status == "Closed")
            return Conflict("No se puede actualizar: la oferta/periodo está cerrado.");

        // Unicidad por (offeringId, Name)
        var dup = await _db.AssessmentTypes.AnyAsync(x =>
            x.SubjectOfferingId == a.SubjectOfferingId &&
            x.Name == dto.Name &&
            x.Id != id, ct);
        if (dup) return Conflict("Ya existe un rubro con ese nombre en la misma oferta.");

        a.Name = dto.Name;
        a.Weight = dto.Weight;
        await _db.SaveChangesAsync(ct);

        // Validar suma de pesos post-actualización
        await _svc.ValidateWeightsAsync(a.SubjectOfferingId);
        return NoContent();
    }

    // ELIMINAR rubro (si no hay notas asociadas) + validar pesos
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var a = await _db.AssessmentTypes
            .Include(x => x.SubjectOffering).ThenInclude(o => o.Period)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (a is null) return NotFound();

        if (a.SubjectOffering.IsClosed || a.SubjectOffering.Period.Status == "Closed")
            return Conflict("No se puede eliminar: la oferta/periodo está cerrado.");

        var hasGrades = await _db.Grades.AnyAsync(g => g.AssessmentTypeId == id, ct);
        if (hasGrades) return Conflict("No se puede eliminar: existen notas asociadas a este rubro.");

        var offeringId = a.SubjectOfferingId;
        _db.AssessmentTypes.Remove(a);
        await _db.SaveChangesAsync(ct);

        // Validar pesos luego de borrar
        await _svc.ValidateWeightsAsync(offeringId);
        return NoContent();
    }
}
