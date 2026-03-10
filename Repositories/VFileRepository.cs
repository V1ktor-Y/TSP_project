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

    public async Task<IEnumerable<VFile>> GetAllAsync() =>
        await _context.Files.ToListAsync();

    public async Task<VFile> GetAsync(int id) =>
        (await _context.Files.Where(f => f.FileId == id).ToListAsync()).FirstOrDefault()!;

    public async Task<IEnumerable<VFile?>> GetByNameAsync(string name) =>
        await _context.Files.Where(f => f.FileName!.Contains(name)).ToListAsync();
}