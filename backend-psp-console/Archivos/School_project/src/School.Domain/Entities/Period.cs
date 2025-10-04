using School.Domain.Common;

namespace School.Domain.Entities;

public class Period : BaseEntity
{
    public string Name { get; set; } = null!;   // "2025-P1"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Open"; // Open/Closed
}
