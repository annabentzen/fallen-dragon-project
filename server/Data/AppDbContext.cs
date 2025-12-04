using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Data;

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
                .LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    public DbSet<PlayerSession> PlayerSessions { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<Act> Acts { get; set; }
    public DbSet<Choice> Choices { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterPose> CharacterPoses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ChoiceHistory> ChoiceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table mappings
        modelBuilder.Entity<Story>().ToTable("Stories");
        modelBuilder.Entity<Act>().ToTable("Acts");
        modelBuilder.Entity<Choice>().ToTable("Choices");
        modelBuilder.Entity<Character>().ToTable("Characters");
        modelBuilder.Entity<CharacterPose>().ToTable("CharacterPoses");
        modelBuilder.Entity<User>().ToTable("Users");

        // Primary keys
        modelBuilder.Entity<User>().HasKey(u => u.UserId);
        modelBuilder.Entity<Choice>().HasKey(c => c.ChoiceId);
        modelBuilder.Entity<Act>().HasKey(a => a.ActId);
        modelBuilder.Entity<Story>().HasKey(s => s.StoryId);

        // ActNumber used as navigation key for story progression
        modelBuilder.Entity<Act>()
            .HasAlternateKey(a => a.ActNumber);

        // Unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // PlayerSession -> CurrentAct (navigates by ActNumber, not ActId)
        modelBuilder.Entity<PlayerSession>(entity =>
        {
            entity.HasOne(ps => ps.CurrentAct)
                .WithMany()
                .HasForeignKey(ps => ps.CurrentActNumber)
                .HasPrincipalKey(a => a.ActNumber)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        // ChoiceHistory tracks player decisions for replay/analytics
        modelBuilder.Entity<ChoiceHistory>(entity =>
        {
            entity.HasIndex(e => e.MadeAt);
            entity.HasIndex(e => e.PlayerSessionId);

            entity.HasOne(e => e.PlayerSession)
                .WithMany(ps => ps.Choices)
                .HasForeignKey(e => e.PlayerSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Choice)
                .WithMany()
                .HasForeignKey(e => e.ChoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User -> PlayerSessions
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.PlayerSessions)
                .WithOne(ps => ps.User)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}