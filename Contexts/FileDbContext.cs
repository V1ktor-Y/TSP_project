using Microsoft.EntityFrameworkCore;
using tsp.Models;

namespace tsp.Contexts;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> dbContextOptions) : base(dbContextOptions)
    { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<VFile> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<VFile>()
            .HasOne(f => f.Account)
            .WithMany(a => a.Files)
            .HasForeignKey(f => f.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
