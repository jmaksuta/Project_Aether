using Azure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectAether.Objects.Models;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Project_Aether_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // --- DbSets for your entities ---

        // Base Game Objects (if you intend to query all game objects regardless of type)
        // With TPH, this will map all GameCharacter derived types into one table.
        public DbSet<GameObject> GameObjects { get; set; } // Recommended to have for the base of TPH

        // Derived Game Characters
        // You generally don't need a DbSet for concrete derived types if you have a DbSet for the base type
        // and are using TPH. However, it doesn't hurt and can sometimes clarify intent.
        // EF Core will still map them to the same table as GameObject.
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<GameContainer> GameContainers { get; set; }
        public DbSet<GameCharacter> GameCharacters { get; set; }
        public virtual DbSet<PlayerCharacter> PlayerCharacters { get; set; }
        public DbSet<NonPlayerCharacter> NonPlayerCharacters { get; set; }

        // DbSet for player profiles (beyond basic identity)
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        // ApplicationUser is handled by IdentityDbContext

        public DbSet<WorldZone> WorldZones { get; set; }

        public DbSet<ArchetypeDefinition> Archetypes { get; set; }

        public DbSet<OnlineConnection> OnlineConnections { get; set; }

        // --- Store and Transactions ---
        public DbSet<StoreTransaction> StoreTransactions { get; set; }
        public DbSet<StoreTransactionItem> StoreTransactionItems { get; set; }
        public DbSet<StoreItem> StoreItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //// Example: Define relationships and constraints here.
            //builder.Entity<GameObject>()
            //    .HasDiscriminator<GameObjectType>("ObjectType")
            //    //.HasValue<GameObject>(GameObjectType.Character) // default fallback
            //    .HasValue<GameObject>(GameObjectType.Portal)
            //    .HasValue<GameObject>(GameObjectType.Door)
            //    .HasValue<GameObject>(GameObjectType.Floor)
            //    .HasValue<GameObject>(GameObjectType.Wall)
            //    .HasValue<GameObject>(GameObjectType.Item)
            //    .HasValue<PlayerCharacter>(GameObjectType.PlayerCharacter)
            //    .HasValue<NonPlayerCharacter>(GameObjectType.NonPlayerCharacter)
            //    .HasValue<GameContainer>(GameObjectType.Container);

            // --- GameObject inheritance mapping (TPH) --- 
            builder.Entity<GameObject>()
                .HasDiscriminator<GameObjectType>("GameObjectType") // Use a string discriminator for better readability
                .HasValue<GameObject>(GameObjectType.UNKNOWN) // Explicitly map base GameObject if it's concrete
                                                              //.HasValue<GameCharacter>(GameObjectType.GameCharacter.ToString()) // Add this if you want GameCharacter to be a concrete type itself
                .HasValue<InventoryItem>(GameObjectType.Item)
                .HasValue<GameContainer>(GameObjectType.Container)
                .HasValue<GameCharacter>(GameObjectType.Character)
                .HasValue<PlayerCharacter>(GameObjectType.PlayerCharacter)
                .HasValue<NonPlayerCharacter>(GameObjectType.NonPlayerCharacter);

            // Relationships

            // --- Inventory and InventoryItem (One-to-Many) ---
            builder.Entity<Inventory>()
                .HasMany(i => i.Items)
                .WithOne(ii => ii.Inventory)
                .HasForeignKey(ii => ii.InventoryId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete items when inventory is deleted


            // --- GameOject and WorldZone (Many-to-One) ---
            builder.Entity<GameObject>()
                .HasOne(go => go.WorldZone)
                .WithMany(wz => wz.GameObjects)
                .HasForeignKey(go => go.WorldZoneId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of WorldZone if it has GameObjects

            // --- Inventory Item and Game Object (One-to-One) ---
            //builder.Entity<InventoryItem>()
            //    .HasOne(ii => ii.Item)
            //    .WithOne() // An InventoryItem has one GameObject (the actual item), GameObject has no navigation back to InventoryItem directly.
            //    .HasForeignKey<InventoryItem>(ii => ii.GameObjectId)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of GameObject if it's referenced by InventoryItem.

            // --- Game Container and Inventory (One-to-One) ---
            builder.Entity<GameContainer>()
                .HasOne(gc => gc.Inventory)
                .WithOne() // An Inventory has one (or no) GameContainer
                .HasForeignKey<GameContainer>(gc => gc.InventoryId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade delete Inventory when GameContainer is deleted, delete it's inventory too.

            // --- GameCharacter and Inventory (One-to-One) ---
            builder.Entity<GameCharacter>()
                .HasOne(gc => gc.Inventory)
                .WithOne() // An Inventory has one (or no) GameCharacter
                .HasForeignKey<GameCharacter>(gc => gc.InventoryId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade delete Inventory when GameCharacter is deleted, delete it's inventory too.

            // --- PlayerCharacter and PlayerProfile (Many-to-One) ---
            builder.Entity<PlayerCharacter>()
                .HasOne(pc => pc.Player)
                .WithMany(pp => pp.Characters)
                .HasForeignKey(pc => pc.PlayerProfileId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of PlayerProfile if it has characters

            // --- PlayerProfile and ApplicationUser (One-to-One) ---
            builder.Entity<PlayerProfile>()
                .HasOne(pp => pp.User)
                .WithOne(au => au.Player) // An ApplicationUser has one PlayerProfile
                .HasForeignKey<PlayerProfile>(pp => pp.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete PlayerProfile when ApplicationUser is deleted

            // --- ArchetypeDefinition and PlayerCharacter (One-to-One) ---
            builder.Entity<PlayerCharacter>()
                .HasOne(pc => pc.ArchetypeDefinition)
                .WithOne()
                .HasForeignKey<PlayerCharacter>(pc => pc.archetypeDefinitionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // Set ArchetypeDefinitionId to null if the ArchetypeDefinition is deleted

            // --- ArchetypeDefinition and StoreItem (One-to-One) ---
            builder.Entity<ArchetypeDefinition>()
                .HasOne(ad => ad.StoreItem)
                .WithOne()
                .HasForeignKey<ArchetypeDefinition>(ad => ad.StoreItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Set StoreItemId to null if the StoreItem is deleted

            // --- OnlinConnection and ApplicationUser (One-to-One) ---
            builder.Entity<OnlineConnection>()
                .HasOne(oc => oc.User)
                .WithOne()
                .HasForeignKey<OnlineConnection>(oc => oc.UserId);

            // --- StoreTransaction and ApplicationUser (Many-to-One) ---
            builder.Entity<StoreTransaction>()
                .HasOne(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of ApplicationUser if they have transactions

            // --- StoreTransaction and StoreTransactionItem (One-to-Many) ---
            builder.Entity<StoreTransactionItem>()
                .HasOne(sti => sti.StoreTransaction)
                .WithMany()
                .HasForeignKey(sti => sti.StoreTransactionId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete transaction items when transaction is deleted

            // --- StoreTransactionItem and StoreItem (Many-to-One) ---
            builder.Entity<StoreTransactionItem>()
                .HasOne(sti => sti.StoreItem)
                .WithMany()
                .HasForeignKey(sti => sti.StoreItemId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of StoreItem if it's referenced by transaction items

            // --- Configure specific property behaviors ---

            // Make Name properties required and set max length
            builder.Entity<GameObject>()
                .Property(go => go.Name)
                .IsRequired()
                .HasMaxLength(100);

            //builder.Entity<GameCharacter>()
            //    .Property(gc => gc.CharacterClass)
            //    .HasMaxLength(50);

            builder.Entity<PlayerCharacter>()
                .Property(pc => pc.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<GameObject>()
                .Property(go => go.xPosition)
                .IsRequired()
                .HasColumnType("float");

            builder.Entity<GameObject>()
                .Property(go => go.yPosition)
                .IsRequired()
                .HasColumnType("float");

            builder.Entity<GameObject>()
                .Property(go => go.zPosition)
                .IsRequired()
                .HasColumnType("float");

            //builder.Entity<Inventory>()
            //    .Property(i => i.OwnerId) // If this OwnerId is still intended for some use
            //    .HasMaxLength(256); // Assuming it could be a string GUID or similar

            //builder.Entity<InventoryItem>()
            //    .Property(ii => ii.ItemId)
            //    .IsRequired()
            //    .HasMaxLength(100);

            builder.Entity<InventoryItem>()
                .Property(ii => ii.ItemType)
                .HasMaxLength(50);

            builder.Entity<PlayerProfile>()
                .Property(pp => pp.PlayerName)
                .IsRequired()
                .HasMaxLength(100);

            // Configure default values
            builder.Entity<GameCharacter>()
                .Property(gc => gc.Level)
                .HasDefaultValue(1);

            builder.Entity<GameCharacter>()
                .Property(gc => gc.Experience)
                .HasDefaultValue(0);

            builder.Entity<GameCharacter>()
                .Property(gc => gc.Health)
                .HasDefaultValue(100);

            builder.Entity<GameCharacter>()
                .Property(gc => gc.Mana)
                .HasDefaultValue(50);

            builder.Entity<GameObject>()
                .Property(go => go.IsActive)
                .HasDefaultValue(true);

            builder.Entity<GameObject>()
                .Property(go => go.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<InventoryItem>()
                .Property(ii => ii.Quantity)
                .HasDefaultValue(1);

            // Ensure enums are mapped as integers
            builder.Entity<GameObject>()
                .Property(go => go.ObjectType)
                .HasConversion<string>(); // Store enum as string for readability in DB
                                          // Or .HasConversion<int>(); // Store enum as integer (default)

            // Configure Identity specific tables if needed, e.g., max lengths for User fields
            builder.Entity<ApplicationUser>()
                .Property(u => u.DateRegistered)
                .HasDefaultValueSql("GETDATE()"); // Example for SQL Server

            // -------------------------------------------------------

            // Seed initial data
            SeedData(builder);

        }

        private void SeedData(ModelBuilder builder)
        {
            SeedArchetypeData(builder);
        }

        private void SeedArchetypeData(ModelBuilder builder)
        {
            // Seed initial data if necessary
            builder.Entity<ArchetypeDefinition>().HasData(

            //1.The "Streetwise Arcane Mechanic"(80s / 90s Street / Industrial Blend):
            //Lore: Grew up in the forgotten, grimy industrial districts where old steampunk machinery and early Magic - Theory tech from the 80s / 90s era still clunk along.You learned to fix things with a wrench and a basic understanding of Aetheric circuits.
            //Starting Point: A small, cramped workshop(your Node / Forge) in a rundown warehouse district, near a rumbling, mana - powered steam engine.The Static Cascade is immediately apparent here.
            //Starting Job Affinity: Bonus to Alchemy and Arcane Mechanics skills.Access to recipes for rudimentary but reliable 80s - inspired Magic - Tech devices(e.g., a "Boombox Golem" blueprint, a "CRT Scrying Monitor" schematics).
            //Initial Quest: Fix a local district's failing mana conduit system, which is causing blackouts and attracting early-stage Echoes.
            //Dialogue: More street - smart, less formal.
            new ArchetypeDefinition
            {
                Id = 1,
                Name = "Streetwise Arcane Mechanic",
                Description = "Grew up in the forgotten, grimy industrial districts where old steampunk machinery and early Magic-Theory tech from the 80s/90s era still clunk along. You learned to fix things with a wrench and a basic understanding of Aetheric circuits.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null, // Set to 0 or null if no associated StoreItem yet
                StartingAbilities = new List<string> { "Basic Alchemy", "Arcane Mechanics" },
                StartingEquipment = new List<string>
                {
                    "a 'Boombox Golem' blueprint",
                    "a 'CRT Scrying Monitor' schematics"
                }
            },

            //2.The "Corporate Scion / Disgraced Magitech Engineer"(Modern Sci - Fi Blend):
            //Lore: Hailing from a powerful, Magic - Theory corporate family that builds the city's sleek, modern infrastructure. You were trained in cutting-edge Magic-Theory but were recently exiled or discredited due to a scandalous project failure (perhaps an early encounter with the Static Cascade).
            //Starting Point: A minimalist, sleek(but now slightly decrepit) penthouse lab / apartment(your Node / Forge) high in a corporate district, with stunning views of the city.The Static Cascade here is more subtle but affecting high - level systems.
            //Starting Job Affinity: Bonus to Magic - Theory Research and Urban Planning skills.Access to blueprints for advanced but unstable modern Magic - Tech.
            //Initial Quest: Re - establish a secure magically - encrypted network connection to a shadow contact within your former corporation, avoiding corporate security.
            //Dialogue: More formal, perhaps a bit jaded or arrogant.
            new ArchetypeDefinition
            {
                Id = 2,
                Name = "Corporate Scion / Disgraced Magitech Engineer",
                Description = "Hailing from a powerful, Magic-Theory corporate family that builds the city's sleek, modern infrastructure. You were trained in cutting-edge Magic-Theory but were recently exiled or discredited due to a scandalous project failure (perhaps an early encounter with the Static Cascade).",
                AvatarImageId = string.Empty // Set to 0 or null if no avatar image yet
            },

            //3.The "Aether-Touched Outlander"(Mythological / Nature Blend):
            //Lore: Came to the metropolis from the untamed "Wilds" – pockets of ancient, primeval fantasy landscapes that exist just beyond or hidden within the city's boundaries. You have a deeper connection to raw Aether and elemental spirits.
            //Starting Point: A secluded, overgrown "Aether Sanctuary"(your Node / Forge) hidden in a forgotten city park or green zone, overgrown with strange glowing flora.The Static Cascade is manifesting as a slow spiritual decay here.
            //Starting Job Affinity: Bonus to Aetheric Harmony(direct manipulation of Aether) and Elemental Crafting skills.Access to recipes for nature - infused alchemical potions or plant - based magical constructs.
            //Initial Quest: Clear a spiritual corruption that is causing nature spirits in the park to lash out, stemming from the Static Cascade.
            //Dialogue: More intuitive, spiritual, perhaps less familiar with urban customs.
            new ArchetypeDefinition
            {
                Id = 3,
                Name = "Aether-Touched Outlander",
                Description = "Came to the metropolis from the untamed 'Wilds' – pockets of ancient, primeval fantasy landscapes that exist just beyond or hidden within the city's boundaries. You have a deeper connection to raw Aether and elemental spirits.",
                AvatarImageId = string.Empty // Set to 0 or null if no avatar image yet
            },

            //4.The "Arcade Alchemist / Subculture Guru"(Direct 80s / 90s Culture Blend):
            //Lore: Grew up immersed in the underground subcultures that celebrate the 80s / 90s aesthetic – from synth-wave musical guilds to grunge alchemist collectives.You're known for your unique alchemical mixes and your connection to the street art scene.
            //Starting Point: A vibrant, neon - drenched(but slightly decaying) back alley workshop(your Node / Forge) that doubles as a hangout for your crew. Graffiti - covered walls, old arcade machines, and experimental alchemical setups abound.The Static Cascade causes aesthetic glitches and feedback loops in the tech.
            //Starting Job Affinity: Bonus to Alchemical Synthesis and Social / Influence skills.Access to recipes for unique "synth-potions" or "graffiti golem" blueprints.
            //Initial Quest: Help a struggling street artist crew whose magically - infused graffiti is fading due to the Static Cascade, requiring a new alchemical "paint" formula.
            //Dialogue: Casual, cool, uses more slang(80s / 90s inspired, obviously).
            new ArchetypeDefinition
            {
                Id = 4,
                Name = "Arcade Alchemist / Subculture Guru",
                Description = "Grew up immersed in the underground subcultures that celebrate the 80s/90s aesthetic – from synth-wave musical guilds to grunge alchemist collectives. You're known for your unique alchemical mixes and your connection to the street art scene.",
                AvatarImageId = string.Empty // Set to 0 or null if no avatar image yet
            },

            //5.The "Arcane Enforcer"(Fighter Archetype)
            //Lore: You were once part of a city - sanctioned Magical Response Unit(MRU) or a private security firm, trained in close-quarters Aether-combat and the tactical application of Magic-Theory weaponry.You're disciplined and practical, but a past incident involving Static Cascade interference (or perhaps the MRU's rigid methods) led to your discharge or disillusionment.
            //Starting Point: A small, spartan barracks-style Node/Forge in a high-density urban sector, possibly near a deactivated (or malfunctioning) Magic-Theory security checkpoint.
            //Starting Job Affinity: Bonus to Combat Skills (melee and ranged Magic-Theory weapons) and Defense Protocol Research.You might start with a basic enchanted baton or a magically-charged sub-Arcana firearm.
            //Initial Quest: Clear a magically-generated blockade (perhaps Echoes forming into a solid barrier) preventing passage through a key urban choke point.
            //Dialogue: Direct, no-nonsense, speaks with tactical precision. Potential for a jaded or reformed hero arc.
            new ArchetypeDefinition
            {
                Id = 5, // Must explicitly provide the PK for seeded data
                Name = "Arcane Enforcer",
                Description = "You were once part of a city-sanctioned Magical Response Unit (MRU) or a private security firm," +
                    " trained in close-quarters Aether-combat and the tactical application of Magic-Theory weaponry." +
                    " You're disciplined and practical, but a past incident involving Static Cascade interference (or perhaps the MRU's rigid methods) led to your discharge or disillusionment",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                BaseHealth = 100,
                BaseMana = 30,
                StoreItemId = null,
                StartingAbilities = new List<string> { "Basic Melee Attack", "Shield Bash" },
            },

            //6. The "Net-Dancer / Aetheric Infiltrator" (Rogue Archetype)
            //Lore: You thrive in the underbelly of the city, using your unique blend of Aetheric manipulation and understanding of Magic-Theory vulnerabilities to bypass magical security systems, hack into enchanted networks(the "Nether-Net"), or acquire rare alchemical components.You're nimble, stealthy, and prefer to work in the shadows.
            //Starting Point: A hidden Node/Forge within a labyrinthine network of magically-concealed alleys and forgotten service tunnels, deep beneath a bustling commercial district.Its entrance might be disguised by an 80s-era "trap door" or a fake neon sign.
            //Starting Job Affinity: Bonus to Stealth, Lock-Glyph Bypass, and Nether-Net Infiltration skills.You might start with a "Ghostweave" cloak(magically dampens resonance) or a device to temporarily scramble Magic-Theory devices.
            //Initial Quest: Infiltrate a mini-corporate Magic-Theory server room (now corrupted by the Cascade) to retrieve crucial data before it's completely lost.
            //Dialogue: Witty, cynical, knows the street's secrets, uses jargon from the underground digital/magical scene.
            new ArchetypeDefinition
            {
                Id = 6,
                Name = "Net-Dancer / Aetheric Infiltrator",
                Description = "You thrive in the underbelly of the city, using your unique blend of Aetheric manipulation and understanding of Magic-Theory vulnerabilities to bypass magical security systems, hack into enchanted networks(the \"Nether-Net\"), or acquire rare alchemical components.You're nimble, stealthy, and prefer to work in the shadows.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },


            //7. The "Urban Soothsayer / Resonance Reader" (Tarot Fortune Teller Archetype)
            //Lore: You possess a rare form of Aetheric divination, allowing you to read the subtle currents and "fate echoes" of the city through methods like Arcane Tarot, Scrying Pool reflections, or Aether-charged crystal balls.You see glimpses of possible futures and past events, making you an invaluable, albeit cryptic, source of information.The Static Cascade is making your readings chaotic and dangerous.
            //Starting Point: A dimly lit, incense-filled Node/Forge disguised as a quirky 80s/90s-style fortune teller parlor or a dusty occult bookstore, tucked away in an old, eccentric neighborhood. It might have a "Psychic Readings 5¢" sign outside.
            //Starting Job Affinity: Bonus to Divination, Lore Discovery, and Charisma skills.You might start with a unique deck of Arcane Tarot cards that provide minor daily insights.
            //Initial Quest: Decipher a series of fragmented visions related to the Static Cascade, which require you to visit specific, unstable Aetheric points in the city.
            //Dialogue: Poetic, metaphorical, often speaks in riddles or omens.
            new ArchetypeDefinition
            {
                Id = 7,
                Name = "Urban Soothsayer / Resonance Reader",
                Description = "You possess a rare form of Aetheric divination, allowing you to read the subtle currents and \"fate echoes\" of the city through methods like Arcane Tarot, Scrying Pool reflections, or Aether-charged crystal balls.You see glimpses of possible futures and past events, making you an invaluable, albeit cryptic, source of information.The Static Cascade is making your readings chaotic and dangerous.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //8. The "Spirit Channeler / Aetheric Empath" (Cleric Archetype)
            //Lore: You are attuned to the more spiritual side of the city's Magic-Theory, capable of communing with benevolent urban spirits, ancient elemental guardians, or the very "soul" of the metropolis itself. You use your connection to mend, heal, and restore, but the Static Cascade is disrupting these vital connections.
            //Starting Point: A serene, albeit slightly neglected, Node/Forge within a hidden rooftop garden or an abandoned subway station converted into a sacred space, filled with glowing runes and small shrines to forgotten city deities.
            //Starting Job Affinity: Bonus to Healing Spells, Aetheric Cleansing, and Spirit Summoning/Pacification skills. You might start with a holy symbol infused with calming Aether or a chime that soothes distressed spirits.
            //Initial Quest: Purify a fountain or public monument that has become corrupted by the Static Cascade, causing local residents(and spirits) distress.
            //Dialogue: Empathetic, calm, wise, often speaks of balance and harmony.
            new ArchetypeDefinition
            {
                Id = 8,
                Name = "Spirit Channeler / Aetheric Empath",
                Description = "You are attuned to the more spiritual side of the city's Magic-Theory, capable of communing with benevolent urban spirits, ancient elemental guardians, or the very \"soul\" of the metropolis itself. You use your connection to mend, heal, and restore, but the Static Cascade is disrupting these vital connections.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //9. The "Urban Mystic / Aether-Prowler" (Martial Artist Archetype)
            //Lore: You've honed your body and mind to channel raw Aetheric energy through precise movements and martial arts techniques, turning your fists and feet into conduits of magical force. You might belong to an underground dojo hidden in a renovated 80s gym, where ancient techniques are taught alongside modern interpretations of Magic-Theory. The Static Cascade tests your control, making your powers unpredictable.
            //Starting Point: A minimalist but functional Node/Forge within an old, converted fighting dojo or a community recreation center from the 80s/90s era, complete with worn-out punching bags and training mats.
            //Starting Job Affinity: Bonus to Unarmed Combat, Aetheric Flow(self-buffs/elemental strikes), and Discipline skills.You might start with enchanted hand wraps or a rare training manual for an ancient martial form.
            //Initial Quest: Stop a group of aggressive Echoes that are manifesting as phantom martial artists, using their chaotic energy to terrorize a local training ground.
            //Dialogue: Focused, honorable, emphasizes physical and mental control, speaks with a sense of dedication.
            new ArchetypeDefinition
            {
                Id = 9,
                Name = "Urban Mystic / Aether-Prowler",
                Description = "You've honed your body and mind to channel raw Aetheric energy through precise movements and martial arts techniques, turning your fists and feet into conduits of magical force. You might belong to an underground dojo hidden in a renovated 80s gym, where ancient techniques are taught alongside modern interpretations of Magic-Theory. The Static Cascade tests your control, making your powers unpredictable.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //10. The "Shadow Weasel" (Burglar/Thief Archetype)
            //Lore: You honed your skills in navigating the city's hidden passages and bypassing its arcane security systems, often for personal gain or to "liberate" forgotten relics. You're nimble, resourceful, and have a knack for getting into places others can't. Your Resonance allows you to sense weak points in magical wards.
            //Starting Point: A secret Node/Forge tucked away in a forgotten maintenance tunnel or behind a false wall in a seemingly abandoned arcade, perfect for discreet operations.
            //Starting Job Affinity: Bonus to Stealth, Arcane Lockpicking/Disabling, and Resource Scavenging.You might start with a multi-tool enchanted to disrupt minor magical security or a map of hidden shortcuts.
            //Initial Quest: Recover a "lost" alchemical formula from a magically locked-down research facility that's been affected by the Static Cascade.
            //Dialogue: Witty, a bit cheeky, observant, and often speaks in coded language about "jobs" and "scores."
            new ArchetypeDefinition
            {
                Id = 10,
                Name = "Shadow Weasel",
                Description = "You honed your skills in navigating the city's hidden passages and bypassing its arcane security systems, often for personal gain or to \"liberate\" forgotten relics. You're nimble, resourceful, and have a knack for getting into places others can't. Your Resonance allows you to sense weak points in magical wards.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //11. The "Aetheric Amplifier" (Bard/Performer Archetype)
            //Lore: You're a natural showperson, using your innate Resonance to amplify your performances and evoke powerful emotions, or even minor magical effects, through music, voice, or art. You might be a budding synth-rock star, a captivating street magician, or a charismatic underground radio DJ. The Static Cascade causes strange feedback loops in magical sound/light.
            //Starting Point: A vibrant, though somewhat chaotic, Node/Forge located in the back room of a small, neon-lit music club or an old community theater that hosted many 80s/90s shows.
            //Starting Job Affinity: Bonus to Influence/Charisma, Aetheric Resonance Manipulation (for emotional/buff effects), and Audience Engagement.You might start with a magically-charged instrument(e.g., a synth-guitar that literally shreds reality) or a vintage microphone that amplifies magical energy.
            //Initial Quest: Restore the magical sound system of a local venue, which is suffering from Static Cascade interference, to host a crucial community event.
            //Dialogue: Energetic, expressive, often uses performance metaphors, charismatic.
            new ArchetypeDefinition
            {
                Id = 11,
                Name = "Aetheric Amplifier",
                Description = "You're a natural showperson, using your innate Resonance to amplify your performances and evoke powerful emotions, or even minor magical effects, through music, voice, or art. You might be a budding synth-rock star, a captivating street magician, or a charismatic underground radio DJ. The Static Cascade causes strange feedback loops in magical sound/light.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //12. The "Mana-Runner" (Han Solo Archetype)
            //Lore: You're a seasoned "Mana-Runner" (smuggler/courier) who operates on the fringes of the city's Magic-Theory regulations.You pilot a "mana-fueled" vehicle (think a classic 80s muscle car or a clunky, modified hover-van, powered by volatile alchemical mixtures) through dangerous, unpatrolled Aetheric zones, delivering illicit alchemical goods, sensitive data stored in memory crystals, or even "off-grid" passengers.You prioritize personal freedom and profit, but possess a surprising moral compass when truly tested.The Static Cascade is making your routes unpredictable and perilous.
            //Starting Point: Your Node/Forge is a disguised vehicle workshop in a less regulated, industrial district, complete with tools, spare parts, and a slightly battered but reliable mana-runner.
            //Starting Job Affinity: Bonus to Driving/Piloting (Magical Vehicles), Underworld Connections, and Quick Thinking (Combat Evasion). You might start with a custom-modified Magic-Theory device that gives temporary boosts to your vehicle.
            //Initial Quest: Escape a magically-patrolled checkpoint and deliver a crucial (and possibly hot) package to a contact across town, while dealing with Cascade-induced traffic glitches.
            //Dialogue: Sarcastic, street-smart, world-weary but with glints of idealism, distrustful of authority.
            new ArchetypeDefinition
            {
                Id = 12,
                Name = "Mana-Runner",
                Description = "You're a seasoned \"Mana-Runner\" (smuggler/courier) who operates on the fringes of the city's Magic-Theory regulations.You pilot a \"mana-fueled\" vehicle (think a classic 80s muscle car or a clunky, modified hover-van, powered by volatile alchemical mixtures) through dangerous, unpatrolled Aetheric zones, delivering illicit alchemical goods, sensitive data stored in memory crystals, or even \"off-grid\" passengers.You prioritize personal freedom and profit, but possess a surprising moral compass when truly tested.The Static Cascade is making your routes unpredictable and perilous.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //13. The "Aether-Baron" (Lando Calrissian Archetype)
            //Lore: You're a charismatic socialite, entrepreneur, or "fixer" who understands the power of charm, influence, and shrewd deals in the city's intertwined magical and corporate circles.You manage (or used to manage) a high-end establishment – perhaps a magically-gambling den, an exclusive neon-lit club, or a black-market Aetheric trading post.You have a silver tongue and a knack for turning any situation to your advantage, often juggling multiple, sometimes conflicting, alliances.
            //Starting Point: Your Node/Forge is a stylish, albeit currently underperforming, suite within a luxurious (or once-luxurious) building in a ritzy district, still bearing the hallmarks of 80s excess.You have a few loyal, but financially strained, contacts.
            //Starting Job Affinity: Bonus to Charisma/Negotiation, Faction Reputation Gain, and Wealth Generation (Passive Income). You might start with a unique, enchanted piece of formal wear or a contact list that grants early access to certain social events.
            //Initial Quest: Host a high-stakes gathering (perhaps to secure a loan or a favor) where the Static Cascade causes social disruptions and magical glitches, requiring you to subtly manage the chaos.
            //Dialogue: Smooth, charming, diplomatic, uses sophisticated language, always looking for an angle.
            new ArchetypeDefinition
            {
                Id = 13,
                Name = "Aether-Baron",
                Description = "You're a charismatic socialite, entrepreneur, or \"fixer\" who understands the power of charm, influence, and shrewd deals in the city's intertwined magical and corporate circles.You manage (or used to manage) a high-end establishment – perhaps a magically-gambling den, an exclusive neon-lit club, or a black-market Aetheric trading post.You have a silver tongue and a knack for turning any situation to your advantage, often juggling multiple, sometimes conflicting, alliances.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            },

            //14. The "Resonance Prodigy" (Luke Skywalker Archetype)
            //Lore: You were raised in relative obscurity, perhaps in a quiet, isolated corner of the city or even just outside it, seemingly disconnected from the grand Magic-Theory networks.However, you've always felt a strange pull, a deeper connection to the city's Aetheric currents than others.You possess an untapped potential for Arcane Resonance, destined for something greater than your humble beginnings.You are idealistic and driven by a desire to help, but naive about the city's true complexities.
            //Starting Point: Your Node/Forge is a modest, slightly rural-feeling dwelling on the outskirts of the metropolis, perhaps built from salvaged materials that evoke an 80s suburban vibe (think a backyard shed that's secretly a powerful magical antenna). The Static Cascade here is a distant hum, but growing.
            //Starting Job Affinity: Bonus to Aetheric Potential (faster Resonance growth), Intuition/Perception, and Healing/Support Spells.You might start with an ancient, simple magical tool passed down through your family that has a hidden power.
            //Initial Quest: Respond to a distress signal from a small, local community being overwhelmed by a burst of Static Cascade energy, forcing you to tap into your nascent Arcane Resonator powers for the first time.
            //Dialogue: Earnest, hopeful, sometimes uncertain but determined, asks many questions.
            new ArchetypeDefinition
            {
                Id = 14,
                Name = "Resonance Prodigy",
                Description = "You were raised in relative obscurity, perhaps in a quiet, isolated corner of the city or even just outside it, seemingly disconnected from the grand Magic-Theory networks.However, you've always felt a strange pull, a deeper connection to the city's Aetheric currents than others.You possess an untapped potential for Arcane Resonance, destined for something greater than your humble beginnings.You are idealistic and driven by a desire to help, but naive about the city's true complexities.",
                AvatarImageId = string.Empty, // Set to 0 or null if no avatar image yet
                StoreItemId = null
            }

            );

        }
    }

}