using Microsoft.EntityFrameworkCore;
using DragonStory.Models;

public class AppDbContext : DbContext
{
    public DbSet<Story> Stories { get; set; }
    public DbSet<Act> Acts { get; set; }
    public DbSet<Choice> Choices { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
