using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;

namespace MyMvcApp.Data
{
    public class MyMvcAppDbContext : DbContext
    {
        public MyMvcAppDbContext(DbContextOptions<MyMvcAppDbContext> options)
            : base(options)
        {
        }

        // DbSet for Characters table
        public DbSet<Character> Characters { get; set; }

        // DbSet for Power table
        public DbSet<Power> Power { get; set; }

        // Optional: configure relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-many: Character -> Powers
            modelBuilder.Entity<Power>()
                .HasOne(p => p.Character)
                .WithMany(c => c.Power)
                .HasForeignKey(p => p.CharacterId);
        }
    }
}
