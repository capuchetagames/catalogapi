using Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Game> Games { get; set; }
    public DbSet<PlayerLibraryGames> PlayerLibraryGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}