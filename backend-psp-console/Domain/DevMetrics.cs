namespace PSPApp.Domain;

public class DevMetrics
{
    public int LOCs { get; set; }    // Líneas de código
    public int LOcm { get; set; }    // Líneas modificadas (manual o futura integración con git)
    public int LOccs { get; set; }   // Líneas comentadas
    public DateTime RecordedAt { get; set; }
}
