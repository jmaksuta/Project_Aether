using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_Aether_Backend.Models;

namespace Project_Aether_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        // DbSet for player profiles (beyond basic identity)
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //// Example: Define relationships and constraints here.
            builder.Entity<PlayerProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PlayerProfile>(p => p.UserId);

            builder.Entity<InventoryItem>()
                .HasOne(i => i.PlayerProfile)
                .WithMany(p => p.Inventory)
                .HasForeignKey(i => i.PlayerProfileId);
        }
    }
}
