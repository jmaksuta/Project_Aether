﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Project_Aether_Backend.Controllers;
using Project_Aether_Backend.Data;
using Project_Aether_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Project_Aether_Backend_Test
{
    [TestFixture]
    public class PlayerCharacterTest
    {

        private Mock<ApplicationDbContext> _mockContext;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private PlayerCharacterController _controller;

        [SetUp]
        public void Setup()
        {
            // 1. Mock ApplicationDbContext
            _mockContext = new Mock<ApplicationDbContext>();

            ApplicationUser user1 = new ApplicationUser { Id = "user1", UserName = "testuser1" };
            ApplicationUser user2 = new ApplicationUser { Id = "user2", UserName = "testuser2" };

            PlayerProfile user1PlayerProfile = new PlayerProfile { Id = 1, UserId = "user1", User = user1, PlayerName = "Test Player 1" };
            PlayerProfile user2PlayerProfile = new PlayerProfile { Id = 2, UserId = "user2", User = user2, PlayerName = "Test Player 2" };


            // Set up DbContext's DbSet for Players
            var playersData = new List<PlayerCharacter>
            {
                new PlayerCharacter { Id = 1, Name = "Player One", PlayerProfileId = user1PlayerProfile.Id, Player = user1PlayerProfile },
                new PlayerCharacter { Id = 2, Name = "Player Two", PlayerProfileId = user2PlayerProfile.Id, Player = user2PlayerProfile }
            }.AsQueryable();

            var mockPlayersDbSet = new Mock<DbSet<PlayerCharacter>>();
            mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.Provider).Returns(playersData.Provider);
            mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.Expression).Returns(playersData.Expression);
            mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.ElementType).Returns(playersData.ElementType);
            mockPlayersDbSet.As<IQueryable<PlayerCharacter>>().Setup(m => m.GetEnumerator()).Returns(playersData.GetEnumerator());
            // For Async operations, you'd need to mock IDbAsyncEnumerable and IDbAsyncEnumerator
            // This is simplified. For full async mocking, you'd use something like Moq.EntityFrameworkCore.

            _mockContext.Setup(c => c.PlayerCharacters).Returns(mockPlayersDbSet.Object);


            // 2. Mock UserManager
            _mockUserManager = MockUserManager.Create<ApplicationUser>();

            // Set up specific UserManager methods as needed for your tests
            // Example: Mock FindByIdAsync
            _mockUserManager
                .Setup(um => um.FindByIdAsync("user1"))
                .ReturnsAsync(new ApplicationUser { Id = "user1", UserName = "testuser1" });

            _mockUserManager
                .Setup(um => um.FindByIdAsync("user2"))
                .ReturnsAsync(new ApplicationUser { Id = "user2", UserName = "testuser2" });

            _mockUserManager
                .Setup(um => um.FindByIdAsync(It.IsNotIn("user1", "user2"))) // For user not found
                .ReturnsAsync((ApplicationUser)null);


            // 3. Instantiate the controller with the mocked dependencies
            _controller = new PlayerCharacterController(_mockContext.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task GetPlayerCharacters_WithValidUserId_ReturnsOkResultWithPlayer()
        {
            // Arrange (already done in Setup for specific user1)

            // Act
            var result = await _controller.GetPlayerCharacters();

            // Assert
            Assert.That(result, Is.InstanceOf<ActionResult<PlayerCharacter>>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var player = okResult.Value as PlayerCharacter;
            Assert.That(player, Is.Not.Null);
            Assert.That(player.Player.User.Id, Is.EqualTo("user1"));
            Assert.That(player.Name, Is.EqualTo("Player One"));

            _mockUserManager.Verify(um => um.FindByIdAsync("user1"), Times.Once);
            _mockContext.Verify(c => c.PlayerCharacters, Times.Once); // Verify DbSet was accessed
        }

        //[Test]
        //public async Task GetPlayer_WithValidUserId_ReturnsOkResultWithPlayer()
        //{
        //    // Arrange (already done in Setup for specific user1)

        //    // Act
        //    var result = await _controller.GetPlayer("user1");

        //    // Assert
        //    Assert.That(result, Is.InstanceOf<ActionResult<Player>>());
        //    var okResult = result.Result as OkObjectResult;
        //    Assert.That(okResult, Is.Not.Null);
        //    Assert.That(okResult.StatusCode, Is.EqualTo(200));

        //    var player = okResult.Value as Player;
        //    Assert.That(player, Is.Not.Null);
        //    Assert.That(player.UserId, Is.EqualTo("user1"));
        //    Assert.That(player.Name, Is.EqualTo("Player One"));

        //    _mockUserManager.Verify(um => um.FindByIdAsync("user1"), Times.Once);
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
            Assert.AreEqual(2, result.Count);
            Assert.Contains("Hero1", result);
            Assert.Contains("Hero2", result);
        }
    }
}
