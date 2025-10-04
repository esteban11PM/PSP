using School.Application.DTOs;

namespace School.Application.Services.Interfaces;

public interface ISubjectOfferingService
{
    Task<int> CreateAsync(SubjectOfferingRequestDTO dto);
    Task CloseAsync(int offeringId);
    Task<SubjectOfferingResponseDTO?> GetAsync(int id);
}

public interface IEnrollmentService
{
    Task<int> EnrollAsync(EnrollmentRequestDTO dto);
    Task RecalculateFinalAverageAsync(int enrollmentId);
}

public interface IAssessmentService
{
    Task<int> CreateAsync(AssessmentTypeRequestDTO dto);
    Task ValidateWeightsAsync(int subjectOfferingId); // suma = 100
}

public interface IGradeService
{
    Task<int> UpsertAsync(GradeRequestDTO dto); // crea o actualiza
}
