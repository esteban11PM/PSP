using School.Domain.Common;

namespace School.Domain.Entities;

public class Student : BaseEntity
{
    public string DocumentNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Status { get; set; } = "Active"; // Active/Inactive
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
