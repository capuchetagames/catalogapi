using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository;

public class ApplicationDbContext : DbContext
{
    //private string _connectionString = "Server=localhost,1433;Database=Db.Catalog;User Id=sa;Password=rooot1234!#;TrustServerCertificate=True;";
    private string _connectionString;

    public ApplicationDbContext()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        _connectionString = configuration.GetConnectionString("ConnectionString");
    }
    
    public ApplicationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public DbSet<Game> Games { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured) return;
        
        optionsBuilder.UseSqlServer(_connectionString);
        //optionsBuilder.UseSqlServer(_configuration.GetValue<string>("ConnectionStrings:ConnectionString"));
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}