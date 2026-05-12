using tsp.Contexts;
using tsp.Models;
using Microsoft.EntityFrameworkCore;

namespace tsp.Repositories;

public class VFileRepository : IVFileRepository
{
    private readonly FileDbContext _context;
    public VFileRepository(FileDbContext context) => _context = context;

    public async Task AddAsync(VFile file)
    {
        await _context.Files.AddAsync(file);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(VFile file)
    {
        _context.Files.Remove(file);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<VFile>> GetAllAsync(int accountId) =>
        await _context.Files
            .Where(f => f.AccountId == accountId)
            .ToListAsync();

    public async Task<VFile?> GetAsync(int id, int accountId) =>
        await _context.Files.FirstOrDefaultAsync(f => f.FileId == id && f.AccountId == accountId);

    public async Task<IEnumerable<VFile>> GetByNameAsync(string name, int accountId) =>
        await _context.Files
            .Where(f => f.AccountId == accountId && f.FileName!.Contains(name))
            .ToListAsync();
}
