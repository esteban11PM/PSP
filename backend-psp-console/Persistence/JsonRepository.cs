using System.Text.Json;
using PSPApp.Common;
using PSPApp.Domain;

namespace PSPApp.Persistence;

public class JsonRepository : IRepository
{
    private readonly string _filePath;
    private List<PSPTask> _items = new();

    public JsonRepository(string filePath)
    {
        _filePath = filePath;
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<PSPTask>>(json, JsonOptions.Options);
                if (items is not null) _items = items;
            }
            catch
            {
                _items = new();
            }
        }
    }

    public IEnumerable<PSPTask> All() => _items;

    public void Add(PSPTask task) => _items.Add(task);

    public void Save()
    {
        var json = JsonSerializer.Serialize(_items, JsonOptions.Options);
        File.WriteAllText(_filePath, json);
    }
}
