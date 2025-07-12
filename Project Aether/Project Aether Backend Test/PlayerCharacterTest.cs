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
using System.Security.Claims;

namespace Project_Aether_Backend_Test
{
    [TestFixture]
    public class PlayerCharacterTest
    {

        private Mock<ApplicationDbContext> _mockContext;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private PlayerCharacterController _controller;

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
            
            // Pass the options to the Mock constructor
            _mockContext = new Mock<ApplicationDbContext>(options); // <--- CHANGE IS HERE

            // 1. Mock ApplicationDbContext
            //_mockContext = new Mock<ApplicationDbContext>();

            ApplicationUser user1 = new ApplicationUser { Id = USER1, UserName = TEST_USER_1 };
            ApplicationUser user2 = new ApplicationUser { Id = USER2, UserName = TEST_USER_2 };

            Project_Aether_Backend.Models.PlayerProfile user1PlayerProfile = new Project_Aether_Backend.Models.PlayerProfile { Id = 1, ApplicationUserId = USER1, ApplicationUser = user1, PlayerName = TEST_PLAYER_1 };
            Project_Aether_Backend.Models.PlayerProfile user2PlayerProfile = new Project_Aether_Backend.Models.PlayerProfile { Id = 2, ApplicationUserId = USER2, ApplicationUser = user2, PlayerName = TEST_PLAYER_2 };

            User user1_2_1 = UserMapper.ToSharedUser(user1);
            User user2_2_1 = UserMapper.ToSharedUser(user2);

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

            _mockContext.Setup(c => c.PlayerCharacters).ReturnsDbSet(playersData); // <--- HUGE SIMPLIFICATION HERE!


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
            _controller = new PlayerCharacterController(_mockContext.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task GetPlayerCharacters_ValidUserId()
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
            var result = await _controller.GetPlayerCharacters();

            // Assert
            Assert.That(result, Is.InstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>());
            //Assert.That(result, Is.InstanceOf<ActionResult<PlayerCharacter>>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var players = okResult.Value as List<PlayerCharacter>;
            var playerChar1 = (players != null) ? players[0] : new PlayerCharacter() as PlayerCharacter;
            Assert.That(playerChar1, Is.Not.Null);
            Assert.That(playerChar1.Player.User.Id, Is.EqualTo(USER1));
            Assert.That(playerChar1.Name, Is.EqualTo(PLAYER_CHARACTER_1_1));

            var playerChar2 = (players != null) ? players[1] : new PlayerCharacter() as PlayerCharacter;
            Assert.That(playerChar2, Is.Not.Null);
            Assert.That(playerChar2.Player.User.Id, Is.EqualTo(USER1));
            Assert.That(playerChar2.Name, Is.EqualTo(PLAYER_CHARACTER_1_2));

            _mockUserManager.Verify(um => um.FindByIdAsync(USER1), Times.Never);
            _mockContext.Verify(c => c.PlayerCharacters, Times.AtLeastOnce); // Verify DbSet was accessed
        }

        [Test]
        public async Task GetPlayerCharacterById_ValidUserId()
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
            var result = await _controller.GetPlayerCharacterById("1");

            // Assert
            Assert.That(result, Is.InstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>());
            //Assert.That(result, Is.InstanceOf<ActionResult<PlayerCharacter>>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var player = okResult.Value as PlayerCharacter;
            Assert.That(player, Is.Not.Null);
            Assert.That(player.Player.User.Id, Is.EqualTo(USER1));
            Assert.That(player.Name, Is.EqualTo(PLAYER_CHARACTER_1_1));

            _mockUserManager.Verify(um => um.FindByIdAsync(USER1), Times.Never);
            _mockContext.Verify(c => c.PlayerCharacters, Times.AtLeastOnce); // Verify DbSet was accessed
        }

        [Test]
        public async Task GetPlayerCharacterById_InvalidUserId()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, USER2),
                    new Claim(ClaimTypes.Name, TEST_USER_2)
                }))
                }
            };
            // Act
            var result = await _controller.GetPlayerCharacterById("1");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var okResult = result as NotFoundObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(404));

            _mockUserManager.Verify(um => um.FindByIdAsync(USER1), Times.Never);
            _mockContext.Verify(c => c.PlayerCharacters, Times.AtLeastOnce); // Verify DbSet was accessed
        }

        //[Test]
        //public async Task GetPlayer_WithValidUserId_ReturnsOkResultWithPlayer()
        //{
        //    // Arrange (already done in Setup for specific user1)

        //    // Act
        //    var result = await _controller.GetPlayer(USER1);

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<ActionResult<Player>>());
        //    var okResult = result.Result as OkObjectResult;
        //    Assert.That(okResult, Is.Not.Null);
        //    Assert.That(okResult.StatusCode, Is.EqualTo(200));

        //    var player = okResult.Value as Player;
        //    Assert.That(player, Is.Not.Null);
        //    Assert.That(player.UserId, Is.EqualTo(USER1));
        //    Assert.That(player.Name, Is.EqualTo("Player One"));

        //    _mockUserManager.Verify(um => um.FindByIdAsync(USER1), Times.Once);
        //    _mockContext.Verify(c => c.Players, Times.Once); // Verify DbSet was accessed
        //}

        //[Test]
        //public async Task GetPlayer_WithNonExistentUserId_ReturnsNotFound()
        //{
        //    // Arrange (userManager is setup to return null for other IDs)

        //    // Act
        //    var result = await _controller.GetPlayer("nonexistent_user");

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<ActionResult<Player>>());
        //    Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        //    var notFoundResult = result.Result as NotFoundResult;
        //    Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));

        //    _mockUserManager.Verify(um => um.FindByIdAsync("nonexistent_user"), Times.Once);
        //    // No need to verify _mockContext.Players because the user isn't found first
        //}

        //[Test]
        //public async Task GetPlayer_WithUserIdHavingNoPlayerProfile_ReturnsNotFound()
        //{
        //    // Arrange: Mock a user that exists in UserManager but doesn't have a Player profile
        //    _mockUserManager
        //        .Setup(um => um.FindByIdAsync("user_with_no_profile"))
        //        .ReturnsAsync(new ApplicationUser { Id = "user_with_no_profile", UserName = "no_profile_user" });

        //    // Ensure the Players DbSet setup won't find this user's profile
        //    var emptyPlayersData = new List<Player>().AsQueryable();
        //    var mockEmptyPlayersDbSet = new Mock<DbSet<Player>>();
        //    mockEmptyPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.Provider).Returns(emptyPlayersData.Provider);
        //    mockEmptyPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.Expression).Returns(emptyPlayersData.Expression);
        //    mockEmptyPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.ElementType).Returns(emptyPlayersData.ElementType);
        //    mockEmptyPlayersDbSet.As<IQueryable<Player>>().Setup(m => m.GetEnumerator()).Returns(emptyPlayersData.GetEnumerator());

        //    _mockContext.Setup(c => c.Players).Returns(mockEmptyPlayersDbSet.Object);


        //    // Act
        //    var result = await _controller.GetPlayer("user_with_no_profile");

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<ActionResult<Player>>());
        //    Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        //    var notFoundResult = result.Result as NotFoundResult;
        //    Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));

        //    _mockUserManager.Verify(um => um.FindByIdAsync("user_with_no_profile"), Times.Once);
        //    _mockContext.Verify(c => c.Players, Times.Once);
        //}

        // Add more tests for other controller actions (e.g., POST, PUT, DELETE)
        // Remember to mock the UserManager methods (CreateAsync, UpdateAsync, DeleteAsync, etc.)
        // and DbContext methods (Add, Update, Remove, SaveChangesAsync) accordingly.


        //[Test]
        //public async Task GetAllItemsAsync_ReturnsAllItems()
        //{
        //    // Arrange
        //    var data = new List<PlayerCharacter>
        //    {
        //        new PlayerCharacter { Id = 1, Name = "Character A" },
        //        new PlayerCharacter { Id = 2, Name = "Character B" }
        //    }.AsQueryable();

        //    var mockSet = new Mock<DbSet<PlayerCharacter>>();
        //    mockSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.Provider).Returns(data.Provider);
        //    mockSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.Expression).Returns(data.Expression);
        //    mockSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.ElementType).Returns(data.ElementType);
        //    mockSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        //    var mockContext = new Mock<ApplicationDbContext>();
        //    mockContext.Setup(c => c.PlayerCharacters).Returns(mockSet.Object);

        //    var repository = new PlayerCharacterController(mockContext.Object, );

        //    // Act
        //    var result = await repository.GetAllItemsAsync();

        //    // Assert
        //    Assert.That(result.Count(), Is.EqualTo(2));
        //    Assert.That(result.First().Name, Is.EqualTo("Item A"));
        //}

        //[Test]
        //public async Task AddItemAsync_AddsItemAndSavesChanges()
        //{
        //    // Arrange
        //    var newItem = new Item { Id = 3, Name = "New Item" };
        //    var mockSet = new Mock<DbSet<Item>>();
        //    var mockContext = new Mock<YourDbContext>();

        //    mockContext.Setup(m => m.Items).Returns(mockSet.Object);
        //    mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1); // Simulate 1 row affected

        //    var repository = new ItemRepository(mockContext.Object);

        //    // Act
        //    await repository.AddItemAsync(newItem);

        //    // Assert
        //    mockSet.Verify(m => m.Add(newItem), Times.Once());
        //    mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        //}

        // Add more unit tests for other repository methods

        [Test]
        public void TestListPlayerCharacters()
        {
            // Arrange
            var playerCharacters = new List<PlayerCharacter>
            {
                new PlayerCharacter { Id = 1, Name = "Hero1", Level = 10 },
                new PlayerCharacter { Id = 2, Name = "Hero2", Level = 20 }
            }.AsQueryable();
            // Act
            var result = playerCharacters.Select(pc => pc.Name).ToList();
            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            //Assert.AreEqual(2, result.Count);
            Assert.Contains("Hero1", result);
            Assert.Contains("Hero2", result);
        }
    }
}
