using PSPApp.Domain;

namespace PSPApp.Persistence;

public interface IRepository
{
    IEnumerable<PSPTask> All();
    void Add(PSPTask task);
    void Save();
}
