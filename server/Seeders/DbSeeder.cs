using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task Seed(AppDbContext context)
    {
         // Ensure foreign keys are enforced
            context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON");

            // Desired story data
            var storyId = 1;
            var storyTitle = "The Fallen Dragon";

            // Full list of acts and choices
            var allActs = new List<Act>
            {
                    // Round 1
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

                    // Round 3
                    new Act
                    {
                        ActNumber = 5,
                        Text = "Checked breathing: Dragon is weak but alive.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Heal with potion", NextActNumber = 12 },
                            new Choice { Text = "Wait for natural recovery", NextActNumber = 13 }
                        }
                    },
                    new Act
                    {
                        ActNumber = 6,
                        Text = "Spoke softly: Dragon opens an eye.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Offer food/water", NextActNumber = 14 },
                            new Choice { Text = "Back away respectfully", NextActNumber = 15 }
                        }
                    },
                    new Act
                    {
                        ActNumber = 7,
                        Text = "Touched scales: Dragon lashes tail weakly.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Hold your ground", NextActNumber = 16 },
                            new Choice { Text = "Retreat in fear", NextActNumber = 17 }
                        }
                    },

                    // Round 4
                    new Act
                    {
                        ActNumber = 8,
                        Text = "Villagers panic.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Fight them off to protect dragon", NextActNumber = 18 },
                            new Choice { Text = "Let them kill it", NextActNumber = -3 } // Bad ending
                        }
                    },
                    new Act
                    {
                        ActNumber = 9,
                        Text = "Villagers bring supplies.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Heal dragon together", NextActNumber = 19 }
                        }
                    },
                    new Act
                    {
                        ActNumber = 10,
                        Text = "Observed patiently.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Notice hunters nearby", NextActNumber = 20 },
                            new Choice { Text = "Notice it heals faintly by itself", NextActNumber = 21 }
                        }
                    },
                    new Act
                    {
                        ActNumber = 11,
                        Text = "Threw rock: Dragon loses trust.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon hostile", NextActNumber = -3 } // Bad ending
                        }
                    },

                    // Round 5
                    new Act
                    {
                        ActNumber = 12,
                        Text = "Dragon glows faintly.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon revives", NextActNumber = -1 } // Good ending
                        }
                    },
                    new Act
                    {
                        ActNumber = 13,
                        Text = "Condition worsens.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon dies", NextActNumber = -3 } // Bad ending
                        }
                    },
                    new Act
                    {
                        ActNumber = 14,
                        Text = "Dragon accepts food/water.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon survives, but weak", NextActNumber = -2 } // Mixed
                        }
                    },
                    new Act
                    {
                        ActNumber = 15,
                        Text = "Backed away respectfully.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon heals on own, distant ally", NextActNumber = -2 } // Mixed
                        }
                    },
                    new Act
                    {
                        ActNumber = 16,
                        Text = "Held your ground: Dragon respects courage.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon bonds", NextActNumber = -1 } // Good
                        }
                    },
                    new Act
                    {
                        ActNumber = 17,
                        Text = "Retreated in fear: Dragon left unguarded.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Hunters find it", NextActNumber = -3 } // Bad
                        }
                    },
                    new Act
                    {
                        ActNumber = 18,
                        Text = "Fought villagers: Risky battle.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon survives but exiled", NextActNumber = -2 } // Mixed
                        }
                    },
                    new Act
                    {
                        ActNumber = 19,
                        Text = "Healed dragon together: Dragon survives.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Villagers trust dragon", NextActNumber = -1 } // Good
                        }
                    },
                    new Act
                    {
                        ActNumber = 20,
                        Text = "Warned dragon of hunters: Dragon flees crippled.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Survives, permanently weakened", NextActNumber = -2 } // Mixed
                        }
                    },
                    new Act
                    {
                        ActNumber = 21,
                        Text = "Dragon self-heals slowly.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Survives, but permanently weakened", NextActNumber = -2 } // Mixed
                        }
                    },
                    new Act
                    {
                        ActNumber = 22,
                        Text = "Dragon hostile after rock: Dies alone.",
                        Choices = new List<Choice>
                        {
                            new Choice { Text = "Dragon dies", NextActNumber = -3 } // Bad
                        }
                    }
            };

           // Load existing story if it exists
            var story = await context.Stories
                .Include(s => s.Acts)
                    .ThenInclude(a => a.Choices)
                .FirstOrDefaultAsync(s => s.StoryId == storyId);

            if (story == null)
            {
                Console.WriteLine("Seeding new story...");
                story = new Story
                {
                    StoryId = storyId,
                    Title = storyTitle,
                    Acts = allActs
                };
                context.Stories.Add(story);
                await context.SaveChangesAsync();
                Console.WriteLine($"Inserted {allActs.Count} acts and their choices.");
            }
            else
            {
                Console.WriteLine("Story exists, checking for missing acts/choices...");

                foreach (var act in allActs)
                {
                    var existingAct = story.Acts.FirstOrDefault(a => a.ActNumber == act.ActNumber);
                    if (existingAct == null)
                    {
                        story.Acts.Add(act);
                        Console.WriteLine($"Added missing act {act.ActNumber}");
                    }
                    else
                    {
                        // Check for missing choices
                        foreach (var choice in act.Choices)
                        {
                            if (!existingAct.Choices.Any(c => c.Text == choice.Text))
                            {
                                existingAct.Choices.Add(choice);
                                Console.WriteLine($"Added missing choice '{choice.Text}' for act {act.ActNumber}");
                            }
                        }
                    }
                }
                await context.SaveChangesAsync();
                Console.WriteLine("Seeding complete: all acts and choices present.");
            }

            // Seed character poses if missing
            if (!context.CharacterPoses.Any())
            {
                context.CharacterPoses.AddRange(
                    new CharacterPose { Id = 1, Name = "Standing", ImageUrl = "pose1.png" },
                    new CharacterPose { Id = 2, Name = "Fighting", ImageUrl = "pose2.png" },
                    new CharacterPose { Id = 3, Name = "Flying", ImageUrl = "pose3.png" }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("Character poses seeded.");
            }

            // Optional: Seed default character for testing
            if (!context.Characters.Any())
            {
                var defaultCharacter = new Character
                {
                    Hair = "Black",
                    Face = "Normal",
                    Outfit = "Adventurer",
                    PoseId = 1
                };
                context.Characters.Add(defaultCharacter);
                await context.SaveChangesAsync();
                Console.WriteLine("Default character seeded.");
            }
        }
    }
