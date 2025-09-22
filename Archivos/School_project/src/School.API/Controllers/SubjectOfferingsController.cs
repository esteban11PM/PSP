using School.API.Extensions;
using School.API.Models;
using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/ofertas-materias")]
public class SubjectOfferingsController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly ISubjectOfferingService _svc;

    public SubjectOfferingsController(SchoolDbContext db, ISubjectOfferingService svc)
    {
        _db = db; _svc = svc;
    }

    // LIST + filtros + paginación
    [HttpGet]
    public async Task<ActionResult<PagedResult<SubjectOfferingResponseDTO>>> List(
        [FromQuery] int? periodId, [FromQuery] int? subjectId, [FromQuery] int? teacherId,
        [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var query = _db.SubjectOfferings
            .AsNoTracking()
            .Include(o => o.Subject)
            .Include(o => o.Period)
            .Include(o => o.Teacher)
            .Include(o => o.Enrollments)
            .AsQueryable();

        if (periodId.HasValue) query = query.Where(o => o.PeriodId == periodId);
        if (subjectId.HasValue) query = query.Where(o => o.SubjectId == subjectId);
        if (teacherId.HasValue) query = query.Where(o => o.TeacherId == teacherId);
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(o => o.Subject.Name.Contains(q) || o.Period.Name.Contains(q));
        }

        var (items, total) = await query
            .OrderByDescending(o => o.Id)
            .Select(o => new SubjectOfferingResponseDTO(
                o.Id,
                o.Subject.Name,
                o.Period.Name,
                o.Teacher == null ? null : (o.Teacher.FirstName + " " + o.Teacher.LastName),
                o.IsClosed,
                o.Enrollments.Count))
            .ToPagedAsync(page, pageSize, ct);

        return Ok(new PagedResult<SubjectOfferingResponseDTO>(items, page, pageSize, total));
    }

    // OBTENER por id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectOfferingResponseDTO>> Get(int id, CancellationToken ct = default)
    {
        var r = await _svc.GetAsync(id);
        return r is null ? NotFound() : Ok(r);
    }

    // CREAR (valida unicidad SubjectId+PeriodId en service)
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] SubjectOfferingRequestDTO dto)
        => Ok(await _svc.CreateAsync(dto));

    // ACTUALIZAR docente asignado (y/o reubicar materia/periodo si lo deseas)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SubjectOfferingRequestDTO dto, CancellationToken ct = default)
    {
        var entity = await _db.SubjectOfferings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        // si cambia SubjectId/PeriodId validamos unicidad
        if (entity.SubjectId != dto.SubjectId || entity.PeriodId != dto.PeriodId)
        {
            var dup = await _db.SubjectOfferings
                .AnyAsync(x => x.SubjectId == dto.SubjectId && x.PeriodId == dto.PeriodId && x.Id != id, ct);
            if (dup) return Conflict("Ya existe esa Materia en ese Periodo.");
        }

        // si periodo/oferta cerrada, bloquear
        var period = await _db.Periods.FirstAsync(p => p.Id == entity.PeriodId, ct);
        if (entity.IsClosed || period.Status == "Closed")
            return Conflict("No se puede actualizar: la oferta o el periodo está cerrado.");

        entity.SubjectId = dto.SubjectId;
        entity.PeriodId = dto.PeriodId;
        entity.TeacherId = dto.TeacherId;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ELIMINAR (si no tiene inscripciones ni rubros)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var o = await _db.SubjectOfferings
            .Include(x => x.AssessmentTypes)
            .Include(x => x.Enrollments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (o is null) return NotFound();

        var p = await _db.Periods.FirstAsync(x => x.Id == o.PeriodId, ct);
        if (o.IsClosed || p.Status == "Closed")
            return Conflict("No se puede eliminar: la oferta/periodo está cerrado.");

        if (o.AssessmentTypes.Any() || o.Enrollments.Any())
            return Conflict("No se puede eliminar: tiene rubros o inscripciones asociadas.");

        _db.SubjectOfferings.Remove(o);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // CERRAR oferta (valida suma de pesos, recalcula definitivas)
    [HttpPost("{id:int}/cerrar")]
    public async Task<IActionResult> Close(int id)
    {
        await _svc.CloseAsync(id);
        return NoContent();
    }

    // RESUMEN (rubros, inscritos, promedio grupo)
    [HttpGet("{id:int}/resumen")]
    public async Task<IActionResult> Summary(int id, CancellationToken ct = default)
    {
        var o = await _db.SubjectOfferings
            .AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.Period)
            .Include(x => x.AssessmentTypes)
            .Include(x => x.Enrollments)
            .ThenInclude(e => e.Grades)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (o is null) return NotFound();

        var avgGroup = o.Enrollments.Any()
            ? Math.Round(o.Enrollments.Where(e => e.FinalAverage.HasValue).Average(e => e.FinalAverage) ?? 0m, 2)
            : 0m;

        return Ok(new
        {
            offeringId = o.Id,
            subject = o.Subject.Name,
            period = o.Period.Name,
            teacherId = o.TeacherId,
            isClosed = o.IsClosed,
            assessments = o.AssessmentTypes.Select(a => new { a.Id, a.Name, a.Weight }),
            enrolled = o.Enrollments.Count,
            groupAverage = avgGroup
        });
    }
}
