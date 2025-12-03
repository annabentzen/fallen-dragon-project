using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;

namespace DragonGame.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await CleanStoryData(context);
        await InsertStory(context);
        await SeedPoses(context);
    }

    private static async Task CleanStoryData(AppDbContext context)
    {
        // Delete in order: children before parents to avoid FK violations
        await context.ChoiceHistories.ExecuteDeleteAsync();
        await context.PlayerSessions.ExecuteDeleteAsync();
        await context.Choices.ExecuteDeleteAsync();
        await context.Acts.ExecuteDeleteAsync();
        await context.Stories.Where(s => s.StoryId == 1).ExecuteDeleteAsync();
    }

    private static async Task InsertStory(AppDbContext context)
    {
        var story = new Story
        {
            StoryId = 1,
            Title = "The Fallen Dragon",
            Acts = new List<Act>
            {
                // ============== ACT 1: First Encounter ==============
                new Act
                {
                    ActNumber = 1,
                    Text = @"Morning light filters through the forest canopy as you search the forest floor for the herbs your mother asked you to fetch, your basket swinging gently by your side.
The air hums with the quiet life of the woods — birdsong from the treetops, the dull thud of stepping on leaves, and the faint scent of yesterday's rain.
Then you see it.
In a clearing ahead lies a large green scaly beast, a pair of leathery wings tucked by its side, smoke rising from its nostrils. A dragon — a creature of legend. Smaller than you might have imagined, young perhaps, but still larger than any animal you have seen before, and unmistakably real.
Its chest rises and falls in slow, shallow breaths, but it is otherwise still. Its eyes closed. It seems to be asleep.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Gently approach the sleeping dragon.", NextActNumber = 11 },
                        new Choice { Text = "Hide and watch behind a nearby bush", NextActNumber = 12 },
                        new Choice { Text = "This is a serious matter, someone needs to know! Run back to the village for help", NextActNumber = 13 }
                    }
                },

                // ============== ACT 11: Approach Branch ==============
                new Act
                {
                    ActNumber = 11,
                    Text = @"A Dragon! This is a sight not even the village elders have seen!
You step softly into the ring of trees, heart hammering at the fear and excitement. The scent of damp earth and singed moss fills the air. You spot some sooted spots on the ground near its head.
The dragon's wings twitch once as you step closer, but it doesn't stir. Its scales shimmer faintly in the dappled light — a metallic sheen, dulled by dirt and specks of crimson.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Maybe you could give it some food? Check your pockets.", NextActNumber = 111 },
                        new Choice { Text = "It could be dangerous to surprise it, try speaking to get its attention.", NextActNumber = 112 },
                        new Choice { Text = "This is an unprecedented opportunity! Get close enough to touch it.", NextActNumber = 113 }
                    }
                },

                // ============== ACT 12: Hide Branch ==============
                new Act
                {
                    ActNumber = 12,
                    Text = @"You crouch down low, and slip behind a nearby bush. From here, the creature's form looks both majestic and terrible.
Its breath comes unevenly — a deep rumble followed by a wheeze. Then you see it: a dark patch of blood staining the grass beneath it.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Gently approach the sleeping dragon, maybe it is wounded?", NextActNumber = 11 },
                        new Choice { Text = "That surely cannot be a real living dragon, right? Pick up and throw a rock to get a reaction.", NextActNumber = 121 },
                        new Choice { Text = "This is too much. Leave as quietly as you can, and pretend nothing happened.", NextActNumber = 122 },
                        new Choice { Text = "This is a serious matter, you've gotta tell someone back home!", NextActNumber = 13 }
                    }
                },

                // ============== ACT 13: Village Branch ==============
                new Act
                {
                    ActNumber = 13,
                    Text = @"You turn and sprint back toward the village, branches and bush whipping at your arms. Someone has to know.
By the time you reach the town square, you're gasping for breath. A couple of villagers glance your way as you clutch your knees, trying to find the words — dragon, alive, sleeping!
But how should this really be handled? Who should you tell first?",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Go look for William - the local hunter - he'll know what to do!", NextActNumber = 131 },
                        new Choice { Text = "This could affect everyone! Find a box, and announce to the town what you've seen!", NextActNumber = 132 }
                    }
                },

                // ============== ACT 111: Food Branch ==============
                new Act
                {
                    ActNumber = 111,
                    Text = @"Your fingers tremble as you rummage through your pockets. The Dragon seems so much larger up close.
Among the crunched up leaves, twine and your lucky rock, you find two pieces of dried jerky — your lunch for the day.
Maybe this could help?
As you glance back toward the dragon, you notice its gaze fixed on you. A low rumble sounds from its throat.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Hold out your hand, offering the jerky to the dragon", NextActNumber = 1111 },
                        new Choice { Text = "Keep your lunch. As a matter of fact, eat it right now.", NextActNumber = 1112 }
                    }
                },

                // ============== ACT 112: Speak Branch ==============
                new Act
                {
                    ActNumber = 112,
                    Text = @"'Hello,'' you whisper, forcing calm into your voice as you raise a hand in slow greeting. The dragon's eyes snap open — molten gold, sharp and alive. It lifts its head and glares, a low rumble rolling from its throat. You spot a line of fresh blood from a gash along its neck, oozing down between its scales.
You freeze, the air thick with tension.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Maybe some food can ease the tension? Check your pockets.", NextActNumber = 111 },
                        new Choice { Text = "Back away, this is not something you can handle by yourself.", NextActNumber = 11112 }
                    }
                },

                // ============== ENDINGS: Bad ==============
                new Act
                {
                    ActNumber = 113,
                    Text = @"Your curiosity gets the better of you, and in a moment of foolish courage, this is a monumental discovery. You have to touch it.
You move forward, step by step. Reaching out your hand. Closer until at last, your fingers make contact with hardened scales.
Immediately its eyes snap open. Pupils narrowed.
There's no time to regret it. A flash of teeth. A blur of motion.
The last thought you have is that this was a horrible idea.",
                    IsEnding = true
                },

                new Act
                {
                    ActNumber = 121,
                    Text = @"The stone leaves your hand before you can think twice. A small *tink* is heard, and the dragon's eyes flare open.
It snarls with an expression you could have sworn looked like panic. A thundering roar echoes in the clearing as it rears back, and a wave of heat and light erupts from its jaws.
You glimpse the gash along its neck just before everything is lost in flame.",
                    IsEnding = true
                },

                new Act
                {
                    ActNumber = 122,
                    Text = @"You turn away and hurry home, heart hammering. By nightfall, you've convinced yourself it was only a trick of light, a fallen tree, a bad trip from a mushroom you ate.
After all, Dragons aren't real.
You do however, just to be sure, avoid that part of the forest in the next couple of months. Months during which there are several instances of people mysteriously getting lost in the woods and never returning.
No one ever finds out why. And, in time, life in the village goes on.",
                    IsEnding = true
                },

                new Act
                {
                    ActNumber = 1112,
                    Text = @"Why should you throw away your own lunch just for this creature? After all, it is your food that you worked hard to pay for.
You pop the jerky into your mouth. The sound of chewing fills the silence.
The dragon looks at you with an expression of surprise that quickly transforms into an angry snarl. A gust of hot air sweeps past you — the dragon's nostrils flare, and a spark ignites deep in its throat.
The last thing you feel is heat and regret.",
                    IsEnding = true
                },

                // ============== ACT 131: Hunter Branch ==============
                new Act
                {
                    ActNumber = 131,
                    Text = @"You rush to the hunter's cottage, at the outskirts of the village, heaving for breath. You knock on the door, but there is no answer. No one is home.
A neighbour that's passing by lets you know she heard William would be out hunting for boar today, and probably will not be back until evening.
Can this really wait?",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Wait for him to return, and then show him the site where you found the dragon.", NextActNumber = 1311 },
                        new Choice { Text = "This is too important to wait, the village HAS to know. Immediately.", NextActNumber = 132 }
                    }
                },

                // ============== ACT 132: Announce Branch ==============
                new Act
                {
                    ActNumber = 132,
                    Text = @"You spend a short moment catching your breath before you scramble onto an old crate in the village square and shout at the top of your lungs.
'Dragon! There's a dragon in the woods!'
The first few faces show disbelief, a couple of people scoff at the ridiculous idea. But you have never been known to lie, and curiosity grows into a murmur.
People gather — farmers, merchants, even children — all listening, expectant, as you recount your trip in the woods and describe the terrible beast slumbering only a couple of hours walk from your homes.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Stir the crowd - feed their fear. Something has to be done!", NextActNumber = 1321 }
                    }
                },

                // ============== ACT 1111: Offer Food ==============
                new Act
                {
                    ActNumber = 1111,
                    Text = @"You hold the jerky out with a shaking hand. The dragon's nostrils flare, drawing in the scent.
Slowly, it stirs — a rumbling sound rising from deep within its chest, not quite a growl, not quite a purr.
It lowers its head, the massive snout brushing your palm as it takes the food delicately between its teeth. The smell of sulphur emanates from its breath, as it swallows the two pieces of jerky - seemingly miniscule in its giant maw - and blinks at you once. Slowly.
For a moment, you're frozen in awe. Then the dragon lifts one wing, revealing a jagged wound along its side and neck — the scales blackened, the flesh raw and glistening. Its golden eyes flick toward you, as if asking for help.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Use what you have on hand to try to tend its wounds.", NextActNumber = 11111 },
                        new Choice { Text = "You've never even seen a dragon before. Whatever is to be done, you need help to do it.", NextActNumber = 11112 }
                    }
                },

                // ============== ENDINGS: Sad (Hunter) ==============
                new Act
                {
                    ActNumber = 1311,
                    Text = @"The sun is sinking when the hunter finally comes home with his bow over his shoulder. He's seemingly had bad luck today and returns empty handed.
At first he scoffs at your story, but agrees to let you lead him into the woods, if only to walk off some pent up frustration.
When you reach the clearing, the dragon is still there — breathing weakly, eyes closed. William's eyes first widen in surprise, and then narrow in focus as he quickly and quietly draws his bow and looses a single arrow. It flies straight and true, piercing the creature's skull.
Silence follows, broken only by the soft rattle of the creature's final breath leaving its body.
When you both go to inspect the beast up close, you discover a large gash from its neck down its side. It was already dying.",
                    IsEnding = true
                },

                // ============== ENDINGS: Worst (Mob) ==============
                new Act
                {
                    ActNumber = 1321,
                    Text = @"Curiosity turns into fear, and spreads faster than reason. Your words spark panic and anger — torches are lit, pitchforks raised. This creature, if it truly exists, will clearly be a threat to the village and cannot be allowed to live.
The crowd surges into the woods, and you are swept along. By the time you reach the clearing, chaos reigns. The wounded dragon thrashes in agony, and the people scream as it lashes out with fire and fangs.
When the smoke clears, several of your friends and neighbours lay dead or wounded around the corpse of the once-majestic beast.",
                    IsEnding = true
                },

                // ============== ACT 11111: Tend Wounds ==============
                new Act
                {
                    ActNumber = 11111,
                    Text = @"You kneel beside the creature, the scent of smoke and blood thick in the air.
The dragon watches every move as you open your pouch of herbs. You grind the leaves between your fingers, releasing their bitter green aroma, and press them gently against the torn scales. It snarls softly, but doesn't strike.
You tear a large strip from your shirt and tie it around the wound.
'It's not much, but this will hopefully stagger the bleeding and dull the pain.' you mumble.
The dragon exhales, eyes half-lidded with exhaustion. Still, you know this isn't enough. The wound runs deep, and you're no healer.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "You have no choice but to do it yourself — no one would believe a story about a dragon anyways.", NextActNumber = 111111 },
                        new Choice { Text = "You can't do this alone — Return to the village for help.", NextActNumber = 11112 }
                    }
                },

                // ============== ACT 11112: Return for Help ==============
                new Act
                {
                    ActNumber = 11112,
                    Text = @"You back away slowly, step by step, until the dragon's form fades into the verdant background.
Then you run — heart pounding, lungs burning — all the way back to the village.
When you burst into the square, the noise and color of daily life feels almost unreal after what you've seen.
But who will believe you? And who can help?",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Find a medical professional — the town healer will surely know what to do.", NextActNumber = 111121 },
                        new Choice { Text = "A beast of that size will be dangerous — fetch William, the local hunter.", NextActNumber = 111122 },
                        new Choice { Text = "Find something to stand on and yell so everyone can hear you. The village must act together.", NextActNumber = 111123 }
                    }
                },

                // ============== ACT 111111: Solo Care ==============
                new Act
                {
                    ActNumber = 111111,
                    Text = @"It might not be much, but it will be better than nothing.
Days become weeks.
Each morning you slip away with food and herbs, pretending to gather supplies. The dragon waits, weak but alive, eyes glinting in the dim light as you clean and wrap its wound.
Over time, fear and awe turns to quiet understanding. Sometimes it hums — a low vibrating sound that feels more like thunder than music.
It doesn't take long before its scales gleam again.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Continue tending its wounds", NextActNumber = 1111211 }
                    }
                },

                // ============== ACT 111121: Healer ==============
                new Act
                {
                    ActNumber = 111121,
                    Text = @"The healer's cottage smells of incense and smoke. Shelves bow under the weight of old books and glass jars filled with roots and powders.
The town healer — a wiry man with kind eyes and an impressive beard reaching down to his tummy — peers up as you stumble in.
Your words a jumble, he hands you a cup of tea, and asks you to take a sip and calm down before you tell your story.
'A dragon?' he repeats, incredulous. But when you describe the wound, his expression softens. 'Show me.' He says as he reaches for a satchel of herbs, ointments and bandages.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Lead him to the dragon", NextActNumber = 1111211 }
                    }
                },

                // ============== ACT 111122: Hunter (Again) ==============
                new Act
                {
                    ActNumber = 111122,
                    Text = @"You rush to the hunter's cottage, at the outskirts of the village, heaving for breath. You knock on the door, but there is no answer. No one is home.
A neighbour that's passing by lets you know that she heard William would be out hunting for boar today, and will probably not be back until evening.
Can this really wait?",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "Yes it can - Wait for him to return.", NextActNumber = 1111221 },
                        new Choice { Text = "No it can't — alert the village.", NextActNumber = 111123 }
                    }
                },

                // ============== ACT 111123: Village Announcement ==============
                new Act
                {
                    ActNumber = 111123,
                    Text = @"You spend a short moment catching your breath before you scramble onto an old crate in the village square and shout at the top of your lungs.
'Dragon! There's a dragon in the woods!'
The first few faces show disbelief, a couple of people scoff at the ridiculous idea. But you have never been known to lie, and curiosity grows into a murmur.
People gather — farmers, merchants, even children — all listening, expectant, as you recount your trip and describe the wounded beast slumbering only a few hours walk from your homes.",
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "The dragon did not attack you - urge the villagers to help the dragon", NextActNumber = 1111231 },
                        new Choice { Text = "Stir the crowd - feed their fear. Something has to be done!", NextActNumber = 1111232 }
                    }
                },

                // ============== ENDINGS: Good (Recovery) ==============
                new Act
                {
                    ActNumber = 1111211,
                    Text = @"With your help, the dragon recovers quickly, and by the end of the month what was once a terrible wound is now simply a jagged scar.
One morning, when you arrive with fresh bandages, it greets you with a low rumble of gratitude. Its eyes are clear now, bright as polished gold.
It nudges your shoulder gently, spreading its wings for the first time since you found it. The wind that rises nearly knocks you backward.
With a final look of farewell — it launches into the sky.
Where it once lay and bled, the earth blossoms months later with strange, silver-green flowers no one has ever seen before.",
                    IsEnding = true
                },

                // ============== ENDINGS: Best (Guardian) ==============
                new Act
                {
                    ActNumber = 1111231,
                    Text = @"The villagers scatter to gather what they can — meat, clean cloth, medicinal herbs, and most importantly: courage.
You run and fetch the town healer, a wiry man with a lengthy beard who mutters something about madness, but follows quickly once he sees the slew of people.
The dragon tenses and growls upon seeing the crowd approaching, but your voice along with a chunk of fresh mutton calms its suspicion, and with time it lets more people come close.
Bandages are laid, wounds cleaned, the great creature breathing evenly again under your watch.
By the next week it is strong enough to walk a short distance by itself, and with everyone working together, you manage to transport it to the outskirts of the village, where it is easier to look after and tend to. In its eyes you can see trust growing for every passing day.
After a month the dragon is soaring through the air again.
In the coming years it becomes a core part of the village. Guarding the townsfolk, and assisting in finding anyone who gets lost in the forest.",
                    IsEnding = true
                },

                // ============== ENDINGS: Sad (Hunter Kills) ==============
                new Act
                {
                    ActNumber = 1111221,
                    Text = @"The sun is sinking when the hunter finally comes home with his bow over his shoulder. He seemingly had bad luck today and returned empty handed.
At first he scoffs at your story, but agrees to let you lead him into the woods, if only to walk off some pent up frustration.
When you arrive, the dragon lies still, breathing shallowly. William's eyes first widen in surprise, then narrow in focus.
You open your mouth to speak — but before you can form a word, an arrow is already soaring through the air. The arrow finds its mark. The dragon jerks once, exhales, and is no more.
You stare at its lifeless form, wondering whether mercy or fear guided your hand in bringing him here.",
                    IsEnding = true
                },

                // ============== ENDINGS: Worst (Mob Violence) ==============
                new Act
                {
                    ActNumber = 1111232,
                    Text = @"Curiosity turns into fear, and spreads faster than reason. Your words spark panic and anger — torches are lit, pitchforks raised.
This creature, if it truly exists, will clearly be a threat to the village and cannot be allowed to live.
The crowd surges into the woods, and you are swept along. By the time you reach the clearing, chaos reigns. The wounded dragon thrashes in agony, and the people scream as it lashes out with fire and fangs.
When the smoke clears, several of your friends and neighbours lay dead or wounded around the corpse of the once-majestic beast.",
                    IsEnding = true
                }
            }
        };

        context.Stories.Add(story);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPoses(AppDbContext context)
    {
        if (await context.CharacterPoses.AnyAsync())
            return;

        var poses = new List<CharacterPose>
        {
            new() { Name = "Rogue Dizzy", ImageUrl = "rogue1-pose1.png", CharacterType = "rogue" },
            new() { Name = "Mage Floating", ImageUrl = "mage1-pose1.png", CharacterType = "mage1" },
            new() { Name = "Mage Crouching", ImageUrl = "mage1-pose2.png", CharacterType = "mage1" },
            new() { Name = "Mage Dizzy", ImageUrl = "mage1-pose3.png", CharacterType = "mage1" },
            new() { Name = "Mage 2 Jumping", ImageUrl = "mage2-pose1.png", CharacterType = "mage2" },
            new() { Name = "Mage 2 Standing", ImageUrl = "mage2-pose2.png", CharacterType = "mage2" },
            new() { Name = "Knight Attacking", ImageUrl = "knight1-pose1.png", CharacterType = "knight" },
            new() { Name = "Knight Striking", ImageUrl = "knight1-pose2.png", CharacterType = "knight" }
        };

        context.CharacterPoses.AddRange(poses);
        await context.SaveChangesAsync();
    }
} 