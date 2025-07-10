using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Project_Aether_Backend.Data;
using Testcontainers.MsSql;

namespace Project_Aether_Backend_Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private MsSqlContainer _msSqlContainer; // If using Testcontainers

        public async Task InitializeAsync()
        {
            // If using Testcontainers for a real SQL Server instance
            _msSqlContainer = new MsSqlBuilder().Build();
            await _msSqlContainer.StartAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a test-specific DbContext
                // Option 1: In-memory database (faster, but not true SQL Server integration)
                // services.AddDbContext<ApplicationDbContext>(options =>
                // {
                //     options.UseInMemoryDatabase("TestDatabase");
                // });

                // Option 2: Testcontainers for a real SQL Server instance
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(_msSqlContainer.GetConnectionString());
                });

                // Build the service provider to get a DbContext instance
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    // Ensure the database is created and seeded for each test run
                    db.Database.EnsureDeleted(); // Start with a clean slate
                    db.Database.EnsureCreated();

                    // Seed your test data here if needed
                    // Example: db.Users.Add(new User { Id = 1, Username = "testuser" });
                    // db.SaveChanges();
                }
            });
        }

        public new async Task DisposeAsync()
        {
            if (_msSqlContainer != null)
            {
                await _msSqlContainer.DisposeAsync();
            }
        }
    }
}
