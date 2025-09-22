using School.Domain.Common;

namespace School.Domain.Entities;

public class Subject : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public byte? WeeklyHours { get; set; }
}
