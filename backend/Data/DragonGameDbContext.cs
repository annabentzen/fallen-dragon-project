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

        // DbSet for CharacterPoses table
        public DbSet<CharacterPose> CharacterPoses { get; set; }

        // to configure relationships
        // In DragonGameDbContext.cs OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing Power configuration
            modelBuilder.Entity<Power>()
                .HasOne(p => p.Character)
                .WithMany(c => c.Power)
                .HasForeignKey(p => p.CharacterId);

            // Character to CharacterPose relationship
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Pose)        // A Character has one Pose
                .WithMany(cp => cp.Characters) // A Pose can be associated with many Characters (via the 'Characters' collection in CharacterPose)
                .HasForeignKey(c => c.PoseId) // The foreign key in Character is 'PoseId'
                .IsRequired(false); // Make the foreign key optional (matching int? PoseId in Character)
        }
    }
}
