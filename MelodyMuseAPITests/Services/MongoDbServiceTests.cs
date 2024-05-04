using Microsoft.VisualStudio.TestTools.UnitTesting;
using MelodyMuseAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using Moq;
using MongoDB.Bson;
using Xunit;

#nullable disable

namespace MelodyMuseAPI.Services.Tests
{
    [TestClass]
    public class MongoDbServiceTests
    {
        private MongoDbService _service;
        private Mock<IMongoCollection<User>> _mockUserCollection;
        private Mock<IMongoCollection<Track>> _mockTrackCollection;
        private Mock<IGridFSBucket> _mockGridFSBucket;
        private Mock<IMongoDatabase> _mockDatabase;
        private Mock<IMongoClient> _mockClient;

        public MongoDbServiceTests()
        {
            var settings = new MongoDbSettings { ConnectionURI = "mongodb://localhost:27017", DatabaseName = "testDb" };
            var mockSettings = new Mock<IOptions<MongoDbSettings>>();
            mockSettings.Setup(s => s.Value).Returns(settings);

            _mockClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockUserCollection = new Mock<IMongoCollection<User>>();

            _mockClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(_mockDatabase.Object);
            _mockDatabase.Setup(db => db.GetCollection<User>("users", null)).Returns(_mockUserCollection.Object);

            _service = new MongoDbService(mockSettings.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsCorrectUser()
        {
            // Arrange
            var fakeUserId = ObjectId.GenerateNewId().ToString();
            var expectedUser = new User { Id = fakeUserId, Email = "user@example.com" };
            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(_ => _.Current).Returns(new[] { expectedUser });
            mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<System.Threading.CancellationToken>())).Returns(true).Returns(false);

            _mockUserCollection.Setup(x => x.FindSync(It.IsAny<FilterDefinition<User>>(), It.IsAny<FindOptions<User, User>>(), It.IsAny<System.Threading.CancellationToken>()))
                               .Returns(mockCursor.Object);

            // Act
            var result = await _service.GetUserByIdAsync(fakeUserId);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(expectedUser.Email, result.Email);
        }

        [Fact]
        public async Task AddUserAsync_InsertsUserCorrectly()
        {
            // Arrange
            var user = new User { Email = "newuser@example.com" };

            // Act
            await _service.AddUserAsync(user);

            // Assert
            _mockUserCollection.Verify(x => x.InsertOneAsync(It.Is<User>(u => u.Email == "newuser@example.com"), null, default));
        }

        [Fact]
        public async Task UpdateUserPasswordAsync_UpdatesPasswordCorrectly()
        {
            // Arrange
            var userId = ObjectId.GenerateNewId().ToString();
            var newPassword = "newSecurePassword123!";
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.PasswordHash, BCrypt.Net.BCrypt.HashPassword(newPassword));
            var updateResult = new UpdateResult.Acknowledged(1, 1, null);

            _mockUserCollection.Setup(x => x.UpdateOneAsync(filter, It.IsAny<UpdateDefinition<User>>(), null, default)).ReturnsAsync(updateResult);

            // Act
            var result = await _service.UpdateUserPasswordAsync(userId, newPassword);

            // Assert
            Xunit.Assert.True(result);
        }
    }
}