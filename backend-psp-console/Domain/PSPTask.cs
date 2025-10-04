namespace PSPApp.Domain;

public class PSPTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string Name { get; set; } = "";
    public int EstimatedLOC { get; set; }
    public List<string> EstimationRefs { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DevMetrics? DevMetrics { get; set; }
    public DateTime? ClosedAt { get; set; }
}
