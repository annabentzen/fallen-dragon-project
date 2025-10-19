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

        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterPose> CharacterPoses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Seed some sample poses
            modelBuilder.Entity<CharacterPose>().HasData(
                new CharacterPose { Id = 1, Name = "Standing", ImageUrl = "pose1.png" },
                new CharacterPose { Id = 2, Name = "Fighting", ImageUrl = "pose2.png" },
                new CharacterPose { Id = 3, Name = "Flying", ImageUrl = "pose3.png" }
            );
        }
    }
}
