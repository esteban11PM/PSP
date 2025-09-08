namespace PSPApp.Domain;

public class PostMortemReport
{
    public string TaskId { get; set; } = "";
    public string TaskName { get; set; } = "";
    public int EstimatedLOC { get; set; }
    public int RealLOCs { get; set; }
    public int RealLOcm { get; set; }
    public int RealLOccs { get; set; }
    public int AbsoluteError { get; set; }
    public double PercentageError { get; set; }
    public string Observation { get; set; } = "";
}
