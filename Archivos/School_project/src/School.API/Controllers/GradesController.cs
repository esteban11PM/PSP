using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/grades")]
public class GradesController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly IGradeService _svc;
    private readonly IEnrollmentService _enrollmentSvc;

    public GradesController(SchoolDbContext db, IGradeService svc, IEnrollmentService enrollmentSvc)
    {
        _db = db; _svc = svc; _enrollmentSvc = enrollmentSvc;
    }

    // LISTAR notas por matrícula
    [HttpGet("by-enrollment/{enrollmentId:int}")]
    public async Task<IActionResult> ByEnrollment(int enrollmentId, CancellationToken ct = default)
    {
        var items = await _db.Grades.AsNoTracking()
            .Include(g => g.AssessmentType)
            .Where(g => g.EnrollmentId == enrollmentId)
            .Select(g => new GradeResponseDTO(
                g.Id,
                "", // opcional: nombre estudiante si lo necesitas
                g.AssessmentType.Name,
                g.Score,
                g.GradedAt))
            .ToListAsync(ct);
        return Ok(items);
    }

    // OBTENER nota por id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GradeResponseDTO>> GetById(int id, CancellationToken ct = default)
    {
        var g = await _db.Grades.AsNoTracking()
            .Include(x => x.AssessmentType)
            .Include(x => x.Enrollment).ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (g is null) return NotFound();

        return Ok(new GradeResponseDTO(
            g.Id,
            g.Enrollment.Student.FirstName + " " + g.Enrollment.Student.LastName,
            g.AssessmentType.Name,
            g.Score,
            g.GradedAt));
    }

    // UPSERT (crea/actualiza) una nota
    [HttpPost("upsert")]
    public async Task<ActionResult<int>> Upsert([FromBody] GradeRequestDTO dto)
        => Ok(await _svc.UpsertAsync(dto));

    // ELIMINAR nota (si oferta/periodo abierto)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var g = await _db.Grades
            .Include(x => x.Enrollment).ThenInclude(e => e.SubjectOffering).ThenInclude(o => o.Period)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (g is null) return NotFound();

        if (g.Enrollment.SubjectOffering.IsClosed || g.Enrollment.SubjectOffering.Period.Status == "Closed")
            return Conflict("No se puede eliminar: la oferta/periodo está cerrado.");

        var enrollmentId = g.EnrollmentId;
        _db.Grades.Remove(g);
        await _db.SaveChangesAsync(ct);

        // Recalcular definitiva de la matrícula
        await _enrollmentSvc.RecalculateFinalAverageAsync(enrollmentId);
        return NoContent();
    }

    // (Opcional) UPSERT masivo para grilla del front
    public record GradeRow(int EnrollmentId, int AssessmentTypeId, decimal Score);

    [HttpPost("bulk-upsert")]
    public async Task<IActionResult> BulkUpsert([FromBody] IEnumerable<GradeRow> rows, CancellationToken ct = default)
    {
        foreach (var row in rows)
        {
            await _svc.UpsertAsync(new GradeRequestDTO(row.EnrollmentId, row.AssessmentTypeId, row.Score));
        }
        // Recalcular definitivas únicas afectadas
        var enrollmentIds = rows.Select(r => r.EnrollmentId).Distinct();
        foreach (var id in enrollmentIds)
            await _enrollmentSvc.RecalculateFinalAverageAsync(id);

        return NoContent();
    }
}
