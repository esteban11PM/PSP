namespace School.Application.DTOs;

public record SubjectOfferingRequestDTO(int SubjectId, int PeriodId, int? TeacherId);
public record SubjectOfferingResponseDTO(int Id, string SubjectName, string PeriodName, string? TeacherFullName, bool IsClosed, int EnrolledCount);

public record EnrollmentRequestDTO(int SubjectOfferingId, int StudentId);
public record EnrollmentResponseDTO(int Id, string StudentFullName, string SubjectName, string PeriodName, decimal? FinalAverage);

public record AssessmentTypeRequestDTO(int SubjectOfferingId, string Name, byte Weight);
public record AssessmentTypeResponseDTO(int Id, int SubjectOfferingId, string Name, byte Weight);

public record GradeRequestDTO(int EnrollmentId, int AssessmentTypeId, decimal Score);
public record GradeResponseDTO(int Id, string StudentFullName, string AssessmentName, decimal Score, DateTime GradedAt);
