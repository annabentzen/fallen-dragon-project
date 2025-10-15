using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Data

{
    public class DragonGameDbContext : DbContext
    {
         // DbSet for Characters table
        public DbSet<Character> Characters { get; set; }
        // DbSet for CharacterPoses table
        public DbSet<CharacterPose> CharacterPoses { get; set; }


        public DragonGameDbContext(DbContextOptions<DragonGameDbContext> options)
            : base(options)
        {
        }
        
        // to configure relationships
        // In DragonGameDbContext.cs OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Character to CharacterPose relationship
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Pose)        // A Character has one Pose
                .WithMany(cp => cp.Characters) // A Pose can be associated with many Characters (via the 'Characters' collection in CharacterPose)
                .HasForeignKey(c => c.PoseId) // The foreign key in Character is 'PoseId'
                .IsRequired(false); // Make the foreign key optional (matching int? PoseId in Character)
        }
    }
}
