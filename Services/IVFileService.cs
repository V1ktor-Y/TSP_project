using tsp.Models;
namespace tsp.Services;

public interface IVFileService
{
    Task SaveFileAsync(string fileName, byte[] data);
    Task<IEnumerable<VFile>> GetFilesAsync();
    Task<VFile> GetAsync(int id);
    Task<IEnumerable<VFile?>> GetByNameAsync(string name);
}