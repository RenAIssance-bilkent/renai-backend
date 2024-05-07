using MelodyMuseAPI.Models;
using MelodyMuseAPI.Services;
using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

[TestClass]
public class MongoDbServiceTests
{
    private MongoDbService _service;
    private Mock<IMongoClient> _mockClient;
    private Mock<IMongoDatabase> _mockDatabase;
    private Mock<IAsyncCursor<User>> _mockCursor;
    private Mock<IMongoCollection<User>> _mockUserCollection;

    [TestInitialize]
    public void Initialize()
    {
        _mockClient = new Mock<IMongoClient>();
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockUserCollection = new Mock<IMongoCollection<User>>();
        _mockCursor = new Mock<IAsyncCursor<User>>();

        var settings = Options.Create(new MongoDbSettings { ConnectionURI = "mongodb://localhost:27017", DatabaseName = "TestDB" });

        _mockClient.Setup(c => c.GetDatabase(settings.Value.DatabaseName, null)).Returns(_mockDatabase.Object);
        _mockDatabase.Setup(db => db.GetCollection<User>("users", null)).Returns(_mockUserCollection.Object);

        // Setup the cursor to return a sequence of users
        var users = new List<User> { new User { Name = "Test User" } };
        _mockCursor.Setup(_ => _.Current).Returns(users);

        // Setup the cursor to indicate the end of the sequence
        _mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                   .Returns(true)  // First call returns true, data available
                   .Returns(false); // Subsequent call returns false, end of enumeration

        // Setup FindSync to return the mock cursor
        _mockUserCollection.Setup(c => c.FindSync(It.IsAny<FilterDefinition<User>>(),
                                                  It.IsAny<FindOptions<User>>(),
                                                  It.IsAny<CancellationToken>()))
                           .Returns(_mockCursor.Object);

       // _service = new MongoDbService(settings, _mockClient.Object);
    }

    [TestMethod]
    public async Task GetAllUsersAsync_ReturnsUsers()
    {
        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.AreEqual(1, result.Count, "The number of users returned should be 1");
        Assert.AreEqual("Test User", result[0].Name, "The name of the user should match 'Test User'");
    }
}
