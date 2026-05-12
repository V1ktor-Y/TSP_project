using tsp.Models;
using tsp.Repositories;

namespace tsp.Services;

public class VFileService : IVFileService
{
    private readonly IVFileRepository _repository;
    public VFileService(IVFileRepository repository) => _repository = repository;

    public async Task SaveFileAsync(string fileName, byte[] data, int accountId) =>
        await _repository.AddAsync(new VFile { FileName = fileName, FileData = data, AccountId = accountId });

    public async Task DeleteFileAsync(int id, int accountId)
    {
        var file = await _repository.GetAsync(id, accountId);
        if (file != null)
        {
            await _repository.DeleteAsync(file);
        }
    }

    public async Task<IEnumerable<VFile>> GetFilesAsync(int accountId) =>
        await _repository.GetAllAsync(accountId);

    public async Task<VFile?> GetAsync(int id, int accountId) =>
        await _repository.GetAsync(id, accountId);
    public async Task<IEnumerable<VFile>> GetByNameAsync(string name, int accountId) =>
        await _repository.GetByNameAsync(name, accountId);
}
