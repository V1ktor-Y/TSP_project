using tsp.Models;
namespace tsp.Repositories;

public interface IVFileRepository
{
    Task AddAsync(VFile file);
    Task DeleteAsync(VFile file);
    Task<IEnumerable<VFile>> GetAllAsync(int accountId);
    Task<VFile?> GetAsync(int id, int accountId);
    Task<IEnumerable<VFile>> GetByNameAsync(string name, int accountId);
}
