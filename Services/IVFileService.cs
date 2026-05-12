using tsp.Models;
namespace tsp.Services;

public interface IVFileService
{
    Task SaveFileAsync(string fileName, byte[] data, int accountId);
    Task DeleteFileAsync(int id, int accountId);
    Task<IEnumerable<VFile>> GetFilesAsync(int accountId);
    Task<VFile?> GetAsync(int id, int accountId);
    Task<IEnumerable<VFile>> GetByNameAsync(string name, int accountId);
}
