using PSPApp.Persistence;
using PSPApp.Services;
using PSPApp.Presentation;

namespace PSPApp;

public class Program
{
    public static void Main()
    {
        // Wiring manual (sin DI framework para mantenerlo simple)
        var repo = new JsonRepository(Path.Combine(AppContext.BaseDirectory, "data.json"));
        var service = new PSPService(repo);
        var counter = new LineCounter();
        var menu = new Menu(service, counter);

        menu.Run(); // interfaz de consola
    }
}
