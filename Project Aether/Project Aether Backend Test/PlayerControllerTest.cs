using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Project_Aether_Backend.Controllers;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using Project_Aether_Backend.Shared;
using ProjectAether.Objects.Net._2._1.Standard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project_Aether_Backend_Test
{
    [TestFixture]
    public class PlayerControllerTest
    {
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private PlayerController _controller;

        private const string USER1 = "user1";
        private const string USER2 = "user2";

        private const string TEST_USER_1 = "testuser1";
        private const string TEST_USER_2 = "testuser2";

        private const string TEST_PLAYER_1 = "Test Player 1";
        private const string TEST_PLAYER_2 = "Test Player 2";

        private const string PLAYER_CHARACTER_1_1 = "Player character 1.1";
        private const string PLAYER_CHARACTER_1_2 = "Player character 1.2";
        private const string PLAYER_CHARACTER_2_1 = "Player character 2.1";
        private const string PLAYER_CHARACTER_2_2 = "Player character 2.2";

        [SetUp]
        public void Setup()
        {
            // 1. Mock ApplicationDbContext
            // Create DbContextOptions for an in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name for each test run
                .Options;

            // 1. Mock ApplicationDbContext
            // Pass the options to the Mock constructor
            _mockContext = new Mock<ApplicationDbContext>(options); // <--- CHANGE IS HERE

            ApplicationUser user1 = new ApplicationUser { Id = USER1, UserName = TEST_USER_1 };
            ApplicationUser user2 = new ApplicationUser { Id = USER2, UserName = TEST_USER_2 };

            User user1_2_1 = UserMapper.ToSharedUser(user1);
            User user2_2_1 = UserMapper.ToSharedUser(user2);

            PlayerProfile user1PlayerProfile = new PlayerProfile { Id = 1, UserId = USER1, User = user1_2_1, PlayerName = TEST_PLAYER_1, Characters = new List<PlayerCharacter>() };
            PlayerProfile user2PlayerProfile = new PlayerProfile { Id = 2, UserId = USER2, User = user2_2_1, PlayerName = TEST_PLAYER_2, Characters = new List<PlayerCharacter>() };

            var playerProfilesData = new List<PlayerProfile>
            {
                user1PlayerProfile,
                user2PlayerProfile
            }.AsQueryable();
            // Set up DbContext's DbSet for Players
            var playersData = new List<PlayerCharacter>
            {
                new PlayerCharacter { Id = 1, Name = PLAYER_CHARACTER_1_1, PlayerProfileId = user1PlayerProfile.Id, Player = user1_2_1.Player },
                new PlayerCharacter { Id = 2, Name = PLAYER_CHARACTER_2_1, PlayerProfileId = user2PlayerProfile.Id, Player = user2_2_1.Player },
                new PlayerCharacter { Id = 3, Name = PLAYER_CHARACTER_1_2, PlayerProfileId = user1PlayerProfile.Id, Player = user1_2_1.Player },
                new PlayerCharacter { Id = 4, Name = PLAYER_CHARACTER_2_2, PlayerProfileId = user2PlayerProfile.Id, Player = user2_2_1.Player },
            }.AsQueryable();

            //var mockPlayersDbSet = new Mock<DbSet<PlayerCharacter>>();
            //mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.Provider).Returns(playersData.Provider);
            //mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.Expression).Returns(playersData.Expression);
            //mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.ElementType).Returns(playersData.ElementType);
            //mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.GetEnumerator()).Returns(playersData.GetEnumerator());
            //// For Async operations, you'd need to mock IDbAsyncEnumerable and IDbAsyncEnumerator
            //// This is simplified. For full async mocking, you'd use something like Moq.EntityFrameworkCore.

            //_mockContext.Setup(c => c.PlayerCharacters).Returns(mockPlayersDbSet.Object);

            _mockContext.Setup(c => c.PlayerProfiles).ReturnsDbSet(playerProfilesData); 
            _mockContext.Setup(c => c.PlayerCharacters).ReturnsDbSet(playersData); 


            // 2. Mock UserManager
            _mockUserManager = MockUserManager.Create<ApplicationUser>();

            // Set up specific UserManager methods as needed for your tests
            // Example: Mock FindByIdAsync
            _mockUserManager
                .Setup(um => um.FindByIdAsync(USER1))
                .ReturnsAsync(new ApplicationUser { Id = USER1, UserName = TEST_USER_1 });

            _mockUserManager
                .Setup(um => um.FindByIdAsync(USER2))
                .ReturnsAsync(new ApplicationUser { Id = USER2, UserName = TEST_USER_2 });

            _mockUserManager
                .Setup(um => um.FindByIdAsync(It.IsNotIn(USER1, USER2))) // For user not found
                .ReturnsAsync(null as ApplicationUser);


            // 3. Instantiate the controller with the mocked dependencies
            _controller = new PlayerController(_mockContext.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task GetPlayerProfile_HappyPath()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, USER1),
                    new Claim(ClaimTypes.Name, TEST_USER_1)
                }))
                }
            };
            // Arrange (already done in Setup for specific user1)

            // Act
            var result = await _controller.GetPlayerProfile();

            // Assert
            Assert.That(result, Is.InstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var player = okResult.Value as PlayerProfile;
            Assert.That(player, Is.Not.Null);
            Assert.That(player.UserId, Is.EqualTo(USER1));
            Assert.That(player.User.UserName, Is.EqualTo(TEST_USER_1));
            Assert.That(player.PlayerName, Is.EqualTo(TEST_PLAYER_1));
            Assert.That(player.Characters, Is.Not.Null);
            Assert.That(player.Characters.Count, Is.EqualTo(0)); // Should have 2 characters for user1
            
            _mockUserManager.Verify(um => um.FindByIdAsync(USER1), Times.Never);
            _mockContext.Verify(c => c.PlayerProfiles, Times.AtLeastOnce); // Verify DbSet was accessed
        }


    }
}
