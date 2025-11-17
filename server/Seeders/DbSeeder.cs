using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        Console.WriteLine("🚀 Starting DbSeeder...");

        // STEP 1: NUCLEAR CLEAN — Delete ALL story data (safe for dev)
        await CleanStoryData(context);
        
        // STEP 2: Insert FRESH story from scratch
        await InsertFreshStory(context);
        
        // STEP 3: Seed poses (if missing)
        await SeedPoses(context);
        
        Console.WriteLine("✅ Seeding COMPLETE! Story is perfect.");
    }

    // 🔥 NUKE ALL STORY DATA (safe for development)
    private static async Task CleanStoryData(AppDbContext context)
    {
        Console.WriteLine("💣 Cleaning ALL story data...");
        
        // Delete in correct order (choices → acts → story)
        await context.Choices.ExecuteDeleteAsync();
        await context.Acts.ExecuteDeleteAsync();
        var story = await context.Stories.FindAsync(1);
        if (story != null)
        {
            context.Stories.Remove(story);
            await context.SaveChangesAsync();
        }
        
        Console.WriteLine("✅ Story data wiped clean!");
    }

    // 🆕 INSERT PERFECT FRESH STORY
    private static async Task InsertFreshStory(AppDbContext context)
    {
        Console.WriteLine("🌟 Inserting fresh story...");

        var story = new Story
        {
            StoryId = 1,
            Title = "The Fallen Dragon",
            Acts = new List<Act>
            {
                // Act 1 - EXACTLY what you want
                new Act
                {
                    ActNumber = 1,
                    Text = "You find a young dragon collapsed in the mountains.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Approach carefully", NextActNumber = 2 },
                        new Choice { Text = "Run to fetch villagers", NextActNumber = 3 },
                        new Choice { Text = "Hide and watch", NextActNumber = 4 }
                    }
                },

                // Act 2
                new Act
                {
                    ActNumber = 2,
                    Text = "You approached the dragon carefully.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Check its breathing", NextActNumber = 5 },
                        new Choice { Text = "Speak softly to it", NextActNumber = 6 },
                        new Choice { Text = "Touch its scales", NextActNumber = 7 }
                    }
                },

                // Act 3
                new Act
                {
                    ActNumber = 3,
                    Text = "You fetched villagers.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Villagers panic and want to kill it", NextActNumber = 8 },
                        new Choice { Text = "Convince them to help instead", NextActNumber = 9 }
                    }
                },

                // Act 4
                new Act
                {
                    ActNumber = 4,
                    Text = "You hid and observed.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Observe patiently", NextActNumber = 10 },
                        new Choice { Text = "Throw a rock to test it", NextActNumber = 11 }
                    }
                },

                // ... (ALL your other acts exactly as written)
                new Act { ActNumber = 5, Text = "Checked breathing: Dragon is weak but alive.", Choices = new() { new Choice { Text = "Heal with potion", NextActNumber = 12 }, new Choice { Text = "Wait for natural recovery", NextActNumber = 13 } } },
                new Act { ActNumber = 6, Text = "Spoke softly: Dragon opens an eye.", Choices = new() { new Choice { Text = "Offer food/water", NextActNumber = 14 }, new Choice { Text = "Back away respectfully", NextActNumber = 15 } } },
                new Act { ActNumber = 7, Text = "Touched scales: Dragon lashes tail weakly.", Choices = new() { new Choice { Text = "Hold your ground", NextActNumber = 16 }, new Choice { Text = "Retreat in fear", NextActNumber = 17 } } },
                new Act { ActNumber = 8, Text = "Villagers panic.", Choices = new() { new Choice { Text = "Fight them off to protect dragon", NextActNumber = 18 }, new Choice { Text = "Let them kill it", NextActNumber = -3 } } },
                new Act { ActNumber = 9, Text = "Villagers bring supplies.", Choices = new() { new Choice { Text = "Heal dragon together", NextActNumber = 19 } } },
                new Act { ActNumber = 10, Text = "Observed patiently.", Choices = new() { new Choice { Text = "Notice hunters nearby", NextActNumber = 20 }, new Choice { Text = "Notice it heals faintly by itself", NextActNumber = 21 } } },
                new Act { ActNumber = 11, Text = "Threw rock: Dragon loses trust.", Choices = new() { new Choice { Text = "Dragon hostile", NextActNumber = -3 } } },
                new Act { ActNumber = 12, Text = "Dragon glows faintly.", Choices = new() { new Choice { Text = "Dragon revives", NextActNumber = -1 } } },
                new Act { ActNumber = 13, Text = "Condition worsens.", Choices = new() { new Choice { Text = "Dragon dies", NextActNumber = -3 } } },
                new Act { ActNumber = 14, Text = "Dragon accepts food/water.", Choices = new() { new Choice { Text = "Dragon survives, but weak", NextActNumber = -2 } } },
                new Act { ActNumber = 15, Text = "Backed away respectfully.", Choices = new() { new Choice { Text = "Dragon heals on own, distant ally", NextActNumber = -2 } } },
                new Act { ActNumber = 16, Text = "Held your ground: Dragon respects courage.", Choices = new() { new Choice { Text = "Dragon bonds", NextActNumber = -1 } } },
                new Act { ActNumber = 17, Text = "Retreated in fear: Dragon left unguarded.", Choices = new() { new Choice { Text = "Hunters find it", NextActNumber = -3 } } },
                new Act { ActNumber = 18, Text = "Fought villagers: Risky battle.", Choices = new() { new Choice { Text = "Dragon survives but exiled", NextActNumber = -2 } } },
                new Act { ActNumber = 19, Text = "Healed dragon together: Dragon survives.", Choices = new() { new Choice { Text = "Villagers trust dragon", NextActNumber = -1 } } },
                new Act { ActNumber = 20, Text = "Warned dragon of hunters: Dragon flees crippled.", Choices = new() { new Choice { Text = "Survives, permanently weakened", NextActNumber = -2 } } },
                new Act { ActNumber = 21, Text = "Dragon self-heals slowly.", Choices = new() { new Choice { Text = "Survives, but permanently weakened", NextActNumber = -2 } } },
                new Act { ActNumber = 22, Text = "Dragon hostile after rock: Dies alone.", Choices = new() { new Choice { Text = "Dragon dies", NextActNumber = -3 } } }
            }
        };

        context.Stories.Add(story);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"✅ Story seeded! {story.Acts.Count} acts, {story.Acts.Sum(a => a.Choices.Count)} choices");
    }

    // Poses (unchanged)
    private static async Task SeedPoses(AppDbContext context)
    {
        if (!context.CharacterPoses.Any())
        {
            context.CharacterPoses.AddRange(
                new CharacterPose { Id = 1, Name = "Standing", ImageUrl = "pose1.png" },
                new CharacterPose { Id = 2, Name = "Fighting", ImageUrl = "pose2.png" },
                new CharacterPose { Id = 3, Name = "Flying", ImageUrl = "pose3.png" }
            );
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Poses seeded!");
        }
    }
}