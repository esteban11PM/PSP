using PSPApp.Domain;
using PSPApp.Services;

namespace PSPApp.Presentation;

public class Menu
{
    private readonly IPSPService _service;
    private readonly ILineCounter _counter;

    public Menu(IPSPService service, ILineCounter counter)
    {
        _service = service;
        _counter = counter;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== PSP Console ===");
            Console.WriteLine("1) Planificación (crear nueva tarea)");
            Console.WriteLine("2) Desarrollo (registrar métricas reales)");
            Console.WriteLine("3) Post-Mortem (comparar y cerrar tarea)");
            Console.WriteLine("4) Ver historial");
            Console.WriteLine("5) Exportar CSV");
            Console.WriteLine("0) Salir");
            Console.Write("Elige una opción: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1": Planificacion(); break;
                    case "2": Desarrollo(); break;
                    case "3": PostMortem(); break;
                    case "4": VerHistorial(); break;
                    case "5": ExportarCsv(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Opción inválida. Enter para continuar...");
                        Console.ReadLine();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error] {ex.Message}");
                Console.WriteLine("Enter para continuar...");
                Console.ReadLine();
            }
        }
    }

    private void Planificacion()
    {
        Console.Clear();
        Console.WriteLine("=== Planificación ===");
        Console.Write("Nombre de la tarea/proyecto: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("El nombre no puede estar vacío.");

        Console.Write("Estimación de LOC (entero): ");
        if (!int.TryParse(Console.ReadLine(), out int locEstimate) || locEstimate < 0)
            throw new Exception("Estimación inválida.");

        Console.WriteLine("Referencias para la estimación (separadas por ; )");
        Console.Write("Ej: 'Libro X; Artículo Y; Repo Z': ");
        var refsText = Console.ReadLine()?.Trim() ?? "";
        var refs = refsText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var task = _service.CreateTask(name!, locEstimate, refs);
        Console.WriteLine($"\nTarea creada con Id: {task.Id}");
        Console.WriteLine("Enter para continuar...");
        Console.ReadLine();
    }

    private void Desarrollo()
    {
        Console.Clear();
        Console.WriteLine("=== Desarrollo ===");
        var tasks = _service.GetOpenTasks();

        if (tasks.Count == 0)
        {
            Console.WriteLine("No hay tareas abiertas. Crea una en Planificación.");
            Console.WriteLine("Enter para continuar...");
            Console.ReadLine();
            return;
        }

        ShowTasks(tasks);

        Console.Write("Id de la tarea a actualizar: ");
        var id = Console.ReadLine()?.Trim();
        var task = tasks.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (task is null) throw new Exception("Id no encontrado.");

        Console.WriteLine("\n¿Cómo deseas registrar las métricas?");
        Console.WriteLine("1) Manual (LOCs, LOcm, LOccs)");
        Console.WriteLine("2) Automática (contar .cs en carpeta) + LOcm manual");
        Console.Write("Opción: ");
        var choice = Console.ReadLine();

        int locs, loccm, locccs;

        if (choice == "2")
        {
            Console.Write("Ruta de carpeta con código (.cs): ");
            var path = Console.ReadLine()?.Trim() ?? "";
            var result = _counter.CountFromFolder(path);
            Console.WriteLine($"\nAuto-cálculo:");
            Console.WriteLine($"  LOCs  (code)           : {result.LOCs}");
            Console.WriteLine($"  LOccs (comentarios)    : {result.LOccs}");
            Console.WriteLine($"  Blancas (no cuentan)   : {result.Blanks}");

            locs = result.LOCs;
            locccs = result.LOccs;

            Console.Write("Ingresa LOcm (líneas modificadas): ");
            if (!int.TryParse(Console.ReadLine(), out loccm) || loccm < 0)
                throw new Exception("LOcm inválido.");
        }
        else
        {
            Console.Write("LOCs (líneas de código source): ");
            if (!int.TryParse(Console.ReadLine(), out locs) || locs < 0)
                throw new Exception("LOCs inválido.");

            Console.Write("LOcm (líneas modificadas): ");
            if (!int.TryParse(Console.ReadLine(), out loccm) || loccm < 0)
                throw new Exception("LOcm inválido.");

            Console.Write("LOccs (líneas comentadas): ");
            if (!int.TryParse(Console.ReadLine(), out locccs) || locccs < 0)
                throw new Exception("LOccs inválido.");
        }

        _service.RecordDevelopment(task.Id, new DevMetrics
        {
            LOCs = locs,
            LOcm = loccm,
            LOccs = locccs,
            RecordedAt = DateTime.UtcNow
        });

        Console.WriteLine("\nMétricas registradas.");
        Console.WriteLine("Enter para continuar...");
        Console.ReadLine();
    }

    private void PostMortem()
    {
        Console.Clear();
        Console.WriteLine("=== Post-Mortem ===");
        var tasks = _service.GetOpenTasks();

        if (tasks.Count == 0)
        {
            Console.WriteLine("No hay tareas abiertas para cerrar.");
            Console.WriteLine("Enter para continuar...");
            Console.ReadLine();
            return;
        }

        ShowTasks(tasks);
        Console.Write("Id de la tarea a cerrar: ");
        var id = Console.ReadLine()?.Trim();
        var task = tasks.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (task is null) throw new Exception("Id no encontrado.");

        var report = _service.CloseAndAnalyze(task.Id);

        Console.WriteLine("\n=== Resumen Post-Mortem ===");
        Console.WriteLine($"Tarea     : {report.TaskName}");
        Console.WriteLine($"Estimado  : {report.EstimatedLOC} LOC");
        Console.WriteLine($"Real LOCs : {report.RealLOCs} | LOcm: {report.RealLOcm} | LOccs: {report.RealLOccs}");
        Console.WriteLine($"Error abs : {report.AbsoluteError} LOC");
        Console.WriteLine($"Error %   : {report.PercentageError:F2}%");
        Console.WriteLine($"Observación: {report.Observation}");
        Console.WriteLine("\nEnter para continuar...");
        Console.ReadLine();
    }

    private void VerHistorial()
    {
        Console.Clear();
        Console.WriteLine("=== Historial ===");
        var all = _service.GetAll();
        if (all.Count == 0)
        {
            Console.WriteLine("Sin registros.");
        }
        else
        {
            foreach (var t in all.OrderByDescending(x => x.CreatedAt))
            {
                var state = t.ClosedAt is null ? "Abierta" : $"Cerrada ({t.ClosedAt:yyyy-MM-dd HH:mm})";
                var hasDev = t.DevMetrics is not null ? "Sí" : "No";
                Console.WriteLine($"- {t.Id} | {t.Name} | Est:{t.EstimatedLOC} | Dev:{hasDev} | {state}");
            }
        }

        Console.WriteLine("\nEnter para continuar...");
        Console.ReadLine();
    }

    private void ExportarCsv()
    {
        Console.Clear();
        Console.WriteLine("=== Exportar CSV ===");
        var csvPath = Path.Combine(AppContext.BaseDirectory, "psp_export.csv");
        File.WriteAllText(csvPath, _service.ExportCsv());
        Console.WriteLine($"Exportado a: {csvPath}");
        Console.WriteLine("Ábrelo con Excel/Numbers/Sheets.");
        Console.WriteLine("Enter para continuar...");
        Console.ReadLine();
    }

    private void ShowTasks(List<PSPTask> tasks)
    {
        Console.WriteLine("\nTareas abiertas:");
        foreach (var t in tasks)
            Console.WriteLine($"- Id: {t.Id} | {t.Name} | Estimado LOC: {t.EstimatedLOC} | Creado: {t.CreatedAt:yyyy-MM-dd HH:mm}");
    }
}
