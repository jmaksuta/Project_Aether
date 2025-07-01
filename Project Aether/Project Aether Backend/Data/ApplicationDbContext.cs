using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Emit;

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
        public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
        public DbSet<NonPlayerCharacter> NonPlayerCharacters { get; set; }

        // DbSet for player profiles (beyond basic identity)
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        // ApplicationUser is handled by IdentityDbContext


        public DbSet<OnlineConnection> OnlineConnections { get; set; }

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

            builder.Entity<OnlineConnection>()
                .HasOne(oc => oc.User)
                .WithOne()
                .HasForeignKey<OnlineConnection>(oc => oc.UserId);

            // --- Configure specific property behaviors ---

            // Make Name properties required and set max length
            builder.Entity<GameObject>()
                .Property(go => go.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<GameCharacter>()
                .Property(gc => gc.CharacterClass)
                .HasMaxLength(50);

            builder.Entity<PlayerCharacter>()
                .Property(pc => pc.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

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

        }
    }
}
