using tsp.Models;
using tsp.Repositories;

namespace tsp.Services;

public class VFileService : IVFileService
{
    private readonly IVFileRepository _repository;
    public VFileService(IVFileRepository repository) => _repository = repository;

    public async Task SaveFileAsync(string fileName, byte[] data) =>
        await _repository.AddAsync(new VFile { FileName = fileName, FileData = data });

    public async Task<IEnumerable<VFile>> GetFilesAsync() =>
        await _repository.GetAllAsync();

    public async Task<VFile> GetAsync(int id) =>
        await _repository.GetAsync(id);
    public async Task<IEnumerable<VFile?>> GetByNameAsync(string name) =>
        await _repository.GetByNameAsync(name);
}