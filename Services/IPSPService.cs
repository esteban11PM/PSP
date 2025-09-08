using PSPApp.Domain;

namespace PSPApp.Services;

public interface IPSPService
{
    PSPTask CreateTask(string name, int estimatedLoc, List<string> refs);
    void RecordDevelopment(string id, DevMetrics metrics);
    PostMortemReport CloseAndAnalyze(string id);
    List<PSPTask> GetOpenTasks();
    List<PSPTask> GetAll();
    string ExportCsv();
}
