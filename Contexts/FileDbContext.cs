using Microsoft.EntityFrameworkCore;
using tsp.Models;

namespace tsp.Contexts;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> dbContextOptions) : base(dbContextOptions)
    { }

    public DbSet<VFile> Files { get; set; }
}