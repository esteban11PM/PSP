using School.Domain.Common;

namespace School.Domain.Entities;

public class SubjectOffering : BaseEntity
{
    public int SubjectId { get; set; }
    public int PeriodId { get; set; }
    public int? TeacherId { get; set; }
    public bool IsClosed { get; set; }

    public Subject Subject { get; set; } = null!;
    public Period Period { get; set; } = null!;
    public Teacher? Teacher { get; set; }

    public ICollection<AssessmentType> AssessmentTypes { get; set; } = new List<AssessmentType>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
