using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace School.Application.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly SchoolDbContext _db;
    public EnrollmentService(SchoolDbContext db) => _db = db;

    public async Task<int> EnrollAsync(EnrollmentRequestDTO dto)
    {
        var off = await _db.SubjectOfferings.Include(o => o.Period).FirstOrDefaultAsync(o => o.Id == dto.SubjectOfferingId)
                  ?? throw new KeyNotFoundException("Offering no encontrado.");
        if (off.IsClosed || off.Period.Status == "Closed") throw new InvalidOperationException("La oferta/periodo está cerrado.");

        var exists = await _db.Enrollments.AnyAsync(e => e.StudentId == dto.StudentId && e.SubjectOfferingId == dto.SubjectOfferingId);
        if (exists) throw new InvalidOperationException("El estudiante ya está inscrito en esta Materia-Periodo.");

        var entity = new Domain.Entities.Enrollment { StudentId = dto.StudentId, SubjectOfferingId = dto.SubjectOfferingId };
        _db.Enrollments.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task RecalculateFinalAverageAsync(int enrollmentId)
    {
        var enr = await _db.Enrollments
            .Include(e => e.SubjectOffering).ThenInclude(o => o.AssessmentTypes)
            .Include(e => e.Grades)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId) ?? throw new KeyNotFoundException("Matrícula no encontrada.");

        var weights = enr.SubjectOffering.AssessmentTypes.ToDictionary(a => a.Id, a => (int)a.Weight);
        var sumW = weights.Values.Sum();
        if (sumW != 100) throw new InvalidOperationException("Suma de pesos ≠ 100.");

        var avg = enr.Grades.Sum(g => g.Score * weights[g.AssessmentTypeId] / 100m);
        enr.FinalAverage = Math.Round(avg, 2);
        await _db.SaveChangesAsync();
    }
}
