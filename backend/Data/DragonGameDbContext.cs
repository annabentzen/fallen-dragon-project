using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Data
{
    public class DragonGameDbContext : DbContext
    {
        // DbSet for Character and CharacterPose tables
        public DbSet<Character> Characters { get; set; }
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

            modelBuilder.Entity<Character>().ToTable("Characters");
            modelBuilder.Entity<CharacterPose>().ToTable("CharacterPoses");


            // Character to CharacterPose relationship
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Pose)        // A Character has one Pose
                .WithMany(cp => cp.Characters) // A Pose can be associated with many Characters
                .HasForeignKey(c => c.PoseId) // The foreign key in Character is 'PoseId'
                .IsRequired(false); // Make the foreign key optional
        }
    }
}
