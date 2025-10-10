using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Data
{
    public class DragonGameDbContext : DbContext
    {
        public DragonGameDbContext(DbContextOptions<DragonGameDbContext> options)
            : base(options)
        {
        }

        // DbSet for Characters table
        public DbSet<Character> Characters { get; set; }

        // DbSet for Power table
        public DbSet<Power> Power { get; set; }

        // to configure relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-many releations for Character -> Powers
            modelBuilder.Entity<Power>()
                .HasOne(p => p.Character)
                .WithMany(c => c.Power)
                .HasForeignKey(p => p.CharacterId);
        }
    }
}
