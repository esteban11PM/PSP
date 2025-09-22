using School.API.Extensions;
using School.API.Models;
using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/matriculas")]
public class EnrollmentsController : ControllerBase
{
    private readonly SchoolDbContext _db;
    private readonly IEnrollmentService _svc;

    public EnrollmentsController(SchoolDbContext db, IEnrollmentService svc)
    {
        _db = db; _svc = svc;
    }

    // INSCRIBIR
    [HttpPost]
    public async Task<ActionResult<int>> Enroll([FromBody] EnrollmentRequestDTO dto)
        => Ok(await _svc.EnrollAsync(dto));

    // LISTAR por offering
    [HttpGet("por-oferta/{offeringId:int}")]
    public async Task<ActionResult<PagedResult<EnrollmentResponseDTO>>> ByOffering(
        int offeringId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var q = _db.Enrollments.AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.SubjectOffering).ThenInclude(o => o.Subject)
            .Include(e => e.SubjectOffering).ThenInclude(o => o.Period)
            .Where(e => e.SubjectOfferingId == offeringId);

        var (items, total) = await q
            .OrderBy(e => e.Student.LastName).ThenBy(e => e.Student.FirstName)
            .Select(e => new EnrollmentResponseDTO(
                e.Id,
                e.Student.FirstName + " " + e.Student.LastName,
                e.SubjectOffering.Subject.Name,
                e.SubjectOffering.Period.Name,
                e.FinalAverage))
            .ToPagedAsync(page, pageSize, ct);

        return Ok(new PagedResult<EnrollmentResponseDTO>(items, page, pageSize, total));
    }

    // LISTAR por estudiante (+ filtro periodo)
    [HttpGet("por-estudiante/{studentId:int}")]
    public async Task<IActionResult> ByStudent(int studentId, [FromQuery] int? periodId, CancellationToken ct = default)
    {
        var q = _db.Enrollments.AsNoTracking()
            .Include(e => e.SubjectOffering).ThenInclude(o => o.Subject)
            .Include(e => e.SubjectOffering).ThenInclude(o => o.Period)
            .Where(e => e.StudentId == studentId);

        if (periodId.HasValue) q = q.Where(e => e.SubjectOffering.PeriodId == periodId.Value);

        var items = await q
            .OrderByDescending(e => e.Id)
            .Select(e => new EnrollmentResponseDTO(
                e.Id,
                "", // no se requiere nombre aquí
                e.SubjectOffering.Subject.Name,
                e.SubjectOffering.Period.Name,
                e.FinalAverage))
            .ToListAsync(ct);

        return Ok(items);
    }

    // RECALCULAR definitiva puntual
    [HttpPost("{id:int}/recalcular")]
    public async Task<IActionResult> Recalculate(int id)
    {
        await _svc.RecalculateFinalAverageAsync(id);
        return NoContent();
    }

    // ELIMINAR inscripción (si oferta/periodo abierto)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var e = await _db.Enrollments
            .Include(x => x.SubjectOffering).ThenInclude(o => o.Period)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return NotFound();

        if (e.SubjectOffering.IsClosed || e.SubjectOffering.Period.Status == "Closed")
            return Conflict("No se puede eliminar: la oferta/periodo está cerrado.");

        // También borrar notas asociadas (si política lo permite)
        var grades = _db.Grades.Where(g => g.EnrollmentId == id);
        _db.Grades.RemoveRange(grades);

        _db.Enrollments.Remove(e);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
