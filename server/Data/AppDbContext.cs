using Microsoft.EntityFrameworkCore;
using DragonGame.Models;

namespace DragonGame.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Story> Stories { get; set; }
        public DbSet<Act> Acts { get; set; }
        public DbSet<Choice> Choices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }


    // Seed data for Stories, Acts, and Choices. The main story
   
   /*
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Story>().HasData(
        new Story { Id = 1, Title = "The Fallen Dragon" }
            );


            modelBuilder.Entity<Act>().HasData(
        // Round 1
        new Act { Id = 1, StoryId = 1, Text = "You find a young dragon collapsed in the mountains." },
        // Round 2
        new Act { Id = 2, StoryId = 1, Text = "Approach carefully" }, // 2A
        new Act { Id = 3, StoryId = 1, Text = "Run to fetch villagers" }, // 2B
        new Act { Id = 4, StoryId = 1, Text = "Hide and watch" }, // 2C
                                                                  // Round 3
        new Act { Id = 5, StoryId = 1, Text = "Check its breathing" }, // 3A
        new Act { Id = 6, StoryId = 1, Text = "Speak softly to it" }, // 3B
        new Act { Id = 7, StoryId = 1, Text = "Touch its scales" }, // 3C
        new Act { Id = 8, StoryId = 1, Text = "Villagers panic" }, // 3D
        new Act { Id = 9, StoryId = 1, Text = "Convince villagers to help" }, // 3E
        new Act { Id = 10, StoryId = 1, Text = "Observe patiently" }, // 3F
        new Act { Id = 11, StoryId = 1, Text = "Throw a rock to test it" }, // 3G
                                                                            // Round 4
        new Act { Id = 12, StoryId = 1, Text = "Heal with potion" }, // 4A
        new Act { Id = 13, StoryId = 1, Text = "Wait for natural recovery" }, // 4B
        new Act { Id = 14, StoryId = 1, Text = "Offer food/water" }, // 4C
        new Act { Id = 15, StoryId = 1, Text = "Back away respectfully" }, // 4D
        new Act { Id = 16, StoryId = 1, Text = "Hold your ground" }, // 4E
        new Act { Id = 17, StoryId = 1, Text = "Retreat in fear" }, // 4F
        new Act { Id = 18, StoryId = 1, Text = "Fight villagers" }, // 4G
        new Act { Id = 19, StoryId = 1, Text = "Villagers bring supplies" }, // 4H
        new Act { Id = 20, StoryId = 1, Text = "Notice hunters nearby" }, // 4I
        new Act { Id = 21, StoryId = 1, Text = "Notice self-healing" }, // 4J
        new Act { Id = 22, StoryId = 1, Text = "Dragon loses trust from rock" }, // 4K
                                                                                 // Endings
        new Act { Id = 23, StoryId = 1, Text = "Dragon revives → Ending 1 (Good)" }, // 5A
        new Act { Id = 24, StoryId = 1, Text = "Dragon dies → Ending 3 (Bad)" }, // 5B
        new Act { Id = 25, StoryId = 1, Text = "Dragon survives, but weak → Ending 2 (Mixed)" }, // 5C
        new Act { Id = 26, StoryId = 1, Text = "Respectful retreat → Ending 2 (Mixed)" }, // 5D
        new Act { Id = 27, StoryId = 1, Text = "Held ground → Ending 1 (Good)" }, // 5E
        new Act { Id = 28, StoryId = 1, Text = "You retreat → Ending 3 (Bad)" }, // 5F
        new Act { Id = 29, StoryId = 1, Text = "Villager fight → Ending 2 (Mixed)" }, // 5G
        new Act { Id = 30, StoryId = 1, Text = "Supplies help → Ending 1 (Good)" }, // 5H
        new Act { Id = 31, StoryId = 1, Text = "Warn dragon → Ending 2 (Mixed)" }, // 5I
        new Act { Id = 32, StoryId = 1, Text = "Self-healing → Ending 2 (Mixed)" }, // 5J
        new Act { Id = 33, StoryId = 1, Text = "Dragon hostile → Ending 3 (Bad)" } // 5K
    );

            modelBuilder.Entity<Choice>().HasData(
        // Round 1 → Round 2
        new Choice { Id = 1, ActId = 1, Text = "Approach carefully", NextActId = 2 },
        new Choice { Id = 2, ActId = 1, Text = "Run to fetch villagers", NextActId = 3 },
        new Choice { Id = 3, ActId = 1, Text = "Hide and watch", NextActId = 4 },

        // Round 2A → Round 3
        new Choice { Id = 4, ActId = 2, Text = "Check its breathing", NextActId = 5 },
        new Choice { Id = 5, ActId = 2, Text = "Speak softly to it", NextActId = 6 },
        new Choice { Id = 6, ActId = 2, Text = "Touch its scales", NextActId = 7 },

        // Round 2B → Round 3
        new Choice { Id = 7, ActId = 3, Text = "Villagers panic and want to kill", NextActId = 8 },
        new Choice { Id = 8, ActId = 3, Text = "Convince them to help", NextActId = 9 },

        // Round 2C → Round 3
        new Choice { Id = 9, ActId = 4, Text = "Observe patiently", NextActId = 10 },
        new Choice { Id = 10, ActId = 4, Text = "Throw a rock", NextActId = 11 },

        // Round 3 → Round 4
        new Choice { Id = 11, ActId = 5, Text = "Heal with potion", NextActId = 12 },
        new Choice { Id = 12, ActId = 5, Text = "Wait for natural recovery", NextActId = 13 },
        new Choice { Id = 13, ActId = 6, Text = "Offer food/water", NextActId = 14 },
        new Choice { Id = 14, ActId = 6, Text = "Back away respectfully", NextActId = 15 },
        new Choice { Id = 15, ActId = 7, Text = "Hold your ground", NextActId = 16 },
        new Choice { Id = 16, ActId = 7, Text = "Retreat in fear", NextActId = 17 },
        new Choice { Id = 17, ActId = 8, Text = "Fight villagers to protect dragon", NextActId = 18 },
        new Choice { Id = 18, ActId = 8, Text = "Let them kill it", NextActId = 24 }, // Ending 3
        new Choice { Id = 19, ActId = 9, Text = "Villagers bring supplies", NextActId = 19 },
        new Choice { Id = 20, ActId = 10, Text = "Notice hunters nearby", NextActId = 20 },
        new Choice { Id = 21, ActId = 10, Text = "Notice self-healing", NextActId = 21 },
        new Choice { Id = 22, ActId = 11, Text = "Dragon loses trust", NextActId = 22 },

        // Round 4 → Endings
        new Choice { Id = 23, ActId = 12, Text = "Potion worked", NextActId = 23 },
        new Choice { Id = 24, ActId = 13, Text = "Wait too long", NextActId = 24 },
        new Choice { Id = 25, ActId = 14, Text = "Food accepted", NextActId = 25 },
        new Choice { Id = 26, ActId = 15, Text = "Respectful retreat", NextActId = 26 },
        new Choice { Id = 16, ActId = 16, Text = "Held ground", NextActId = 27 },
        new Choice { Id = 28, ActId = 17, Text = "Retreat", NextActId = 28 },
        new Choice { Id = 29, ActId = 18, Text = "Villager fight", NextActId = 29 },
        new Choice { Id = 30, ActId = 19, Text = "Supplies help", NextActId = 30 },
        new Choice { Id = 31, ActId = 20, Text = "Warn dragon", NextActId = 31 },
        new Choice { Id = 32, ActId = 21, Text = "Self-healing", NextActId = 32 },
        new Choice { Id = 33, ActId = 22, Text = "Dragon hostile", NextActId = 33 }
            );
        }

*/

}