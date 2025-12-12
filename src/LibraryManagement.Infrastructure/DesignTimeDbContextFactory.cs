using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
    {
        public LibraryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
            
            // Connection string dla migracji (design-time)
            // Używamy tego samego co w appsettings.json API
            var connectionString = "Host=localhost;Port=5432;Database=librarydb;Username=postgres;Password=postgrespw";
            
            optionsBuilder.UseNpgsql(connectionString);
            
            return new LibraryDbContext(optionsBuilder.Options);
        }
    }
}