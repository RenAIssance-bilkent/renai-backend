using NUnit.Framework;
using Moq;
using MelodyMuseAPI.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MelodyMuseAPI_DotNet8.Data;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Services;

[TestFixture]
public class UserUnitTest
{
    private Mock<IMongoCollection<User>> _mockUserCollection;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        // Mock the user collection
        _mockUserCollection = new Mock<IMongoCollection<User>>();

        // Mock the MongoDB settings
        var mockMongoDbSettings = new Mock<IOptions<MongoDBSettings>>();
        mockMongoDbSettings.Setup(s => s.Value).Returns(new MongoDBSettings
        {
            // Configure your MongoDB settings here
        });

        // Mock the MongoDB client and database, then set up to return the mock collection
        var mockMongoDatabase = new Mock<IMongoDatabase>();
        var mockMongoClient = new Mock<IMongoClient>();
        mockMongoClient.Setup(c => c.GetDatabase(It.IsAny<string>(), null))
                       .Returns(mockMongoDatabase.Object);
        mockMongoDatabase.Setup(db => db.GetCollection<User>(It.IsAny<string>(), null))
                         .Returns(_mockUserCollection.Object);

        // Initialize the service with mocked dependencies
        var mongoDBService = new MongoDBService(mockMongoDbSettings.Object);
        _userService = new UserService(mongoDBService);
    }

    [Test]
    public async Task AddUser_ShouldAddUserSuccessfully()
    {
        // Arrange
        var userRegistrationDto = new UserRegistrationDto
        {
            Username = "TestUser",
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var result = await _userService.RegisterUser(userRegistrationDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(userRegistrationDto.Email, result.Email);
        // Additional assertions as necessary

        // Verify the insertion call was made once
        _mockUserCollection.Verify(
            c => c.InsertOneAsync(It.IsAny<User>(), null, default),
            Times.Once);
    }
}
