using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DragonGame.Data;

/// <summary>
/// Enables EF Core CLI commands (migrations, database updates) at design time.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=App_Data/DragonGame.db");
        return new AppDbContext(optionsBuilder.Options);
    }
}