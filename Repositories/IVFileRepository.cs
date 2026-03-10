using tsp.Models;
namespace tsp.Repositories;

public interface IVFileRepository
{
    Task AddAsync(VFile file);
    Task<IEnumerable<VFile>> GetAllAsync();
    Task<VFile> GetAsync(int id);
    Task<IEnumerable<VFile?>> GetByNameAsync(string name);
}