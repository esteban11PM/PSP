namespace School.Application.DTOs;

public record StudentRequestDTO(string DocumentNumber, string FirstName, string LastName, string? Email, DateTime? BirthDate, string Status);
public record StudentResponseDTO(int Id, string DocumentNumber, string FullName, string? Email, string Status);

public record TeacherRequestDTO(string DocumentNumber, string FirstName, string LastName, string Email, string? Specialty, string Status);
public record TeacherResponseDTO(int Id, string DocumentNumber, string FullName, string Email, string? Specialty, string Status);

public record SubjectRequestDTO(string Code, string Name, byte? WeeklyHours);
public record SubjectResponseDTO(int Id, string Code, string Name, byte? WeeklyHours);

public record PeriodRequestDTO(string Name, DateTime StartDate, DateTime EndDate, string Status);
public record PeriodResponseDTO(int Id, string Name, DateTime StartDate, DateTime EndDate, string Status);
