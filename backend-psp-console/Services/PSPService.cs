using PSPApp.Domain;
using PSPApp.Persistence;

namespace PSPApp.Services;

public class PSPService : IPSPService
{
    private readonly IRepository _repo;

    public PSPService(IRepository repo) => _repo = repo;

    public PSPTask CreateTask(string name, int estimatedLoc, List<string> refs)
    {
        var task = new PSPTask
        {
            Name = name,
            EstimatedLOC = estimatedLoc,
            EstimationRefs = refs
        };
        _repo.Add(task);
        _repo.Save();
        return task;
    }

    public void RecordDevelopment(string id, DevMetrics metrics)
    {
        var task = _repo.All().FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                   ?? throw new Exception("Tarea no encontrada");
        task.DevMetrics = metrics;
        _repo.Save();
    }

    public PostMortemReport CloseAndAnalyze(string id)
    {
        var task = _repo.All().FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                   ?? throw new Exception("Tarea no encontrada");

        if (task.DevMetrics is null)
            throw new Exception("La tarea no tiene métricas de desarrollo registradas.");

        task.ClosedAt = DateTime.UtcNow;
        _repo.Save();

        var absErr = Math.Abs(task.DevMetrics.LOCs - task.EstimatedLOC);
        var pctErr = task.EstimatedLOC == 0 ? 0 : (absErr * 100.0 / task.EstimatedLOC);

        var obs = pctErr switch
        {
            <= 10 => "Excelente estimación (±10%).",
            <= 25 => "Buena, pero puede afinarse.",
            <= 50 => "Diferencia notable; revisa supuestos y granularidad.",
            _ => "Estimación deficiente; descomponer tareas y basar en datos históricos."
        };

        return new PostMortemReport
        {
            TaskId = task.Id,
            TaskName = task.Name,
            EstimatedLOC = task.EstimatedLOC,
            RealLOCs = task.DevMetrics.LOCs,
            RealLOcm = task.DevMetrics.LOcm,
            RealLOccs = task.DevMetrics.LOccs,
            AbsoluteError = absErr,
            PercentageError = pctErr,
            Observation = obs
        };
    }

    public List<PSPTask> GetOpenTasks() => _repo.All().Where(t => t.ClosedAt is null).ToList();
    public List<PSPTask> GetAll() => _repo.All().ToList();

    public string ExportCsv()
    {
        var rows = new List<string> {
            "Id,Name,EstimatedLOC,RealLOCs,LOcm,LOccs,CreatedAt,ClosedAt,Refs"
        };

        foreach (var t in _repo.All().OrderBy(x => x.CreatedAt))
        {
            var locs = t.DevMetrics?.LOCs ?? 0;
            var locm = t.DevMetrics?.LOcm ?? 0;
            var loccs = t.DevMetrics?.LOccs ?? 0;
            var refs = string.Join(" | ", t.EstimationRefs);
            rows.Add($"{t.Id},{Escape(t.Name)},{t.EstimatedLOC},{locs},{locm},{loccs},{t.CreatedAt:o},{t.ClosedAt:o},{Escape(refs)}");
        }
        return string.Join(Environment.NewLine, rows);

        static string Escape(string v) => $"\"{v.Replace("\"", "\"\"")}\"";
    }
}
