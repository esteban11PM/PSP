using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace School.Application.Services.Implementations;

public class GradeService : IGradeService
{
    private readonly SchoolDbContext _db;
    public GradeService(SchoolDbContext db) => _db = db;

    public async Task<int> UpsertAsync(GradeRequestDTO dto)
    {
        if (dto.Score < 0m || dto.Score > 5m)
            throw new ArgumentOutOfRangeException(nameof(dto.Score), "La nota debe estar entre 0.00 y 5.00");

        var enr = await _db.Enrollments
            .Include(e => e.SubjectOffering).ThenInclude(o => o.Period)
            .FirstOrDefaultAsync(e => e.Id == dto.EnrollmentId) ?? throw new KeyNotFoundException("Matrícula no encontrada.");

        if (enr.SubjectOffering.IsClosed || enr.SubjectOffering.Period.Status == "Closed")
            throw new InvalidOperationException("Edición bloqueada. La oferta/periodo está cerrado.");

        // Upsert por clave única (EnrollmentId, AssessmentTypeId)
        var entity = await _db.Grades
            .FirstOrDefaultAsync(g => g.EnrollmentId == dto.EnrollmentId && g.AssessmentTypeId == dto.AssessmentTypeId);

        if (entity is null)
        {
            entity = new Domain.Entities.Grade
            {
                EnrollmentId = dto.EnrollmentId,
                AssessmentTypeId = dto.AssessmentTypeId,
                Score = dto.Score
            };
            _db.Grades.Add(entity);
        }
        else
        {
            entity.Score = dto.Score;
            entity.GradedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return entity.Id;
    }
}
