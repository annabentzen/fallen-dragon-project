using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
                optionsBuilder
                    .UseSqlite("Data Source=App_Data/DragonGame.db")
                    .LogTo(Console.WriteLine, LogLevel.Information); //log SQL and EF operations
                optionsBuilder.EnableSensitiveDataLogging();
        }
    }

        public DbSet<PlayerSession> PlayerSessions { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<Act> Acts { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterPose> CharacterPoses { get; set; }
        public DbSet<ChoiceHistory> ChoiceHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Story>().ToTable("Stories");
            modelBuilder.Entity<Act>().ToTable("Acts");
            modelBuilder.Entity<Choice>().ToTable("Choices");
            modelBuilder.Entity<Character>().ToTable("Characters");
            modelBuilder.Entity<CharacterPose>().ToTable("CharacterPoses");

            modelBuilder.Entity<Choice>().HasKey(c => c.ChoiceId);
            modelBuilder.Entity<Act>().HasKey(a => a.ActId);
            modelBuilder.Entity<Story>().HasKey(s => s.StoryId);

            modelBuilder.Entity<Story>().HasData(
                new Story { StoryId = 1, Title = "Fallen Dragon" }
            );

            modelBuilder.Entity<Act>().HasData(
                new Act { ActId = 1, StoryId = 1, ActNumber = 1, Text = "The dragon awakens..." }
            );

            modelBuilder.Entity<Choice>().HasData(
                new Choice { ChoiceId = 1, ActId = 1, Text = "Go left", NextActNumber = 2 },
                new Choice { ChoiceId = 2, ActId = 1, Text = "Go right", NextActNumber = 3 }
            );

            modelBuilder.Entity<CharacterPose>().HasData(
                new CharacterPose { Id = 1, Name = "Standing", ImageUrl = "pose1.png" },
                new CharacterPose { Id = 2, Name = "Fighting", ImageUrl = "pose2.png" },
                new CharacterPose { Id = 3, Name = "Flying", ImageUrl = "pose3.png" }
            );

            modelBuilder.Entity<PlayerSession>(entity =>
            {
                entity.HasOne<Act>()                        // PlayerSession has one Act
                    .WithMany()                           // Act can be current for many sessions
                    .HasForeignKey(s => s.CurrentActNumber) // FK is CurrentActNumber
                    .HasPrincipalKey(a => a.ActNumber)     // PK on Act is ActNumber
                    .OnDelete(DeleteBehavior.Restrict);    // Don't delete act if used
            });

            modelBuilder.Entity<ChoiceHistory>(entity =>
            {
                entity.HasIndex(e => e.PlayerSessionId);
                entity.HasIndex(e => e.MadeAt);

                entity.HasOne(e => e.PlayerSession)
                    .WithMany() 
                    .HasForeignKey(e => e.PlayerSessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Choice)
                    .WithMany()
                    .HasForeignKey(e => e.ChoiceId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });
        }
    }
}
