using School.Application.DTOs;
using School.Application.Services.Interfaces;
using School.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace School.Application.Services.Implementations;

public class SubjectOfferingService : ISubjectOfferingService
{
    private readonly SchoolDbContext _db;
    public SubjectOfferingService(SchoolDbContext db) => _db = db;

    public async Task<int> CreateAsync(SubjectOfferingRequestDTO dto)
    {
        var exists = await _db.SubjectOfferings.AnyAsync(x => x.SubjectId == dto.SubjectId && x.PeriodId == dto.PeriodId);
        if (exists) throw new InvalidOperationException("La Materia ya está ofrecida en ese Periodo.");

        var entity = new Domain.Entities.SubjectOffering
        {
            SubjectId = dto.SubjectId,
            PeriodId = dto.PeriodId,
            TeacherId = dto.TeacherId,
            IsClosed = false
        };

        _db.SubjectOfferings.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task CloseAsync(int offeringId)
    {
        var off = await _db.SubjectOfferings
            .Include(o => o.AssessmentTypes)
            .Include(o => o.Enrollments).ThenInclude(e => e.Grades)
            .FirstOrDefaultAsync(x => x.Id == offeringId) ?? throw new KeyNotFoundException("Offering no encontrado.");

        // Validar suma de pesos = 100
        var sum = off.AssessmentTypes.Sum(a => (int)a.Weight);
        if (sum != 100) throw new InvalidOperationException("La suma de pesos de los rubros debe ser 100.");

        // Recalcular todos los FinalAverage antes de cerrar
        foreach (var enr in off.Enrollments)
        {
            if (enr.Grades.Count == 0) { enr.FinalAverage = null; continue; }
            var avg = (from g in enr.Grades
                       join a in off.AssessmentTypes on g.AssessmentTypeId equals a.Id
                       select g.Score * a.Weight / 100m).Sum();
            enr.FinalAverage = Math.Round(avg, 2);
        }

        off.IsClosed = true;
        await _db.SaveChangesAsync();
    }

    public async Task<SubjectOfferingResponseDTO?> GetAsync(int id)
    {
        var q = await _db.SubjectOfferings
            .Include(o => o.Subject).Include(o => o.Period).Include(o => o.Teacher)
            .Include(o => o.Enrollments)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (q is null) return null;

        return new SubjectOfferingResponseDTO(
            q.Id, q.Subject.Name, q.Period.Name,
            q.Teacher is null ? null : $"{q.Teacher.FirstName} {q.Teacher.LastName}",
            q.IsClosed, q.Enrollments.Count
        );
    }
}
