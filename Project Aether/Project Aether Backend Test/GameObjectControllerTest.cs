using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Project_Aether_Backend.Data;
using ProjectAether.Objects.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Project_Aether_Backend_Test
{

    [TestFixture]
    public class GameObjectControllerTest
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _factory = new CustomWebApplicationFactory();
            await _factory.InitializeAsync(); // Initialize Testcontainers container
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up database data after each test (optional, but good for isolation)
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                // Example: Clear all users
                db.Users.RemoveRange(db.Users);
                db.SaveChanges();
            }
        }

        [Test]
        public async Task GetUsers_ReturnsOkResultWithUsers()
        {
            // Arrange: Seed some test data directly into the database
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Users.Add(new ApplicationUser { Id = "1", UserName = "testuser1", Email = "test1@example.com" });
                db.Users.Add(new ApplicationUser { Id = "2", UserName = "testuser2", Email = "test2@example.com" });
                await db.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync("/api/users"); // Replace with your actual API endpoint

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var users = await response.Content.ReadFromJsonAsync<List<ApplicationUser>>(); // Assuming you have a User model
            Assert.NotNull(users);
            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users.Any(u => u.UserName == "testuser1"), Is.True);
        }

        [Test]
        public async Task PostUser_CreatesNewUser()
        {
            // Arrange
            var newUser = new ApplicationUser { UserName = "newuser", Email = "newuser@example.com" };
            var content = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Verify the user was actually saved to the database
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var createdUser = await db.Users.SingleOrDefaultAsync(u => u.UserName == "newuser");
                Assert.That(createdUser, Is.Not.Null);
                Assert.That(createdUser.Email, Is.EqualTo("newuser@example.com"));
            }
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            _client.Dispose();
            await _factory.DisposeAsync(); // Dispose Testcontainers container
        }
    }
}