using Microsoft.VisualStudio.TestTools.UnitTesting;
using MelodyMuseAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Models;
using Moq;
using Xunit;
using MongoDB.Bson;

//to remove the potential errors
#nullable disable

namespace MelodyMuseAPI.Services.Tests
{
    [TestClass]
    public class TrackServiceTests
    {
        private Mock<MongoDbService> _mockMongoDbService;
        private Mock<OpenAIApiService> _mockOpenAIApiService;
        private Mock<ModelService> _mockModelService;
        private TrackService _trackService;

        public TrackServiceTests()
        {
            _mockMongoDbService = new Mock<MongoDbService>();
            _mockOpenAIApiService = new Mock<OpenAIApiService>();
            _mockModelService = new Mock<ModelService>();
            _trackService = new TrackService(_mockMongoDbService.Object, _mockOpenAIApiService.Object, _mockModelService.Object);
        }

        [TestMethod]
        public async Task GetAllTracksTest_ReturnsTracks()
        [Fact]
        public async Task GenerateTrack_WithEnoughPoints_Success()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto
            {
                Genre = "Test Genre",
                Model = 0,
                Prompt = "Test Prompt"
            
            };

            var userId = ObjectId.GenerateNewId().ToString();
            var expectedTrackId = ObjectId.GenerateNewId().ToString();
            _mockMongoDbService.Setup(service => service.ReduceUserPoints(userId, It.IsAny<int>())).ReturnsAsync(true);
            _mockMongoDbService.Setup(service => service.AddTrackAsync(It.IsAny<TrackGenerationDto>(), It.IsAny<Stream>(), It.IsAny<Stream>())).ReturnsAsync(expectedTrackId);

            // Act
            var result = await _trackService.GenerateTrack(trackCreationDto, userId);

            // Assert
            Xunit.Assert.Equal(expectedTrackId, result);
        }

        [Fact]
        public async Task GenerateTrack_NotEnoughPoints_ThrowsException()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto
            {
                Genre = "Test Genre",
                Model = 0,
                Prompt = "Test Prompt"
                
            };

            var userId = ObjectId.GenerateNewId().ToString();
            _mockMongoDbService.Setup(service => service.ReduceUserPoints(userId, It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<Exception>(() => _trackService.GenerateTrack(trackCreationDto, userId));
        }

        [Fact]
        public async Task GetAllTracks_ReturnsListOfTracks()
        {
            // Arrange
            var tracks = new List<Track> { new Track { Title = "examle title 1" }, new Track { Title = "example title 2" } };
            _mockMongoDbService.Setup(service => service.GetAllTracksAsync()).ReturnsAsync(tracks);

            // Act
            var result = await _trackService.GetAllTracks();

            // Assert
            Xunit.Assert.Equal(tracks, result);
        }

        [Fact]
        public async Task GetTrackById_ReturnsTrack()
        {
            // Arrange
            var trackId = ObjectId.GenerateNewId().ToString();
            var expectedTrack = new Track { Id = trackId };
            _mockMongoDbService.Setup(service => service.GetTrackByIdAsync(trackId)).ReturnsAsync(expectedTrack);

            // Act
            var result = await _trackService.GetTrackById(trackId);

            // Assert
            Xunit.Assert.Equal(expectedTrack, result);
        }

        [Fact]
        public async Task GetTracksByUser_ReturnsListOfTracks()
        {
            // Arrange
            var user1Id = ObjectId.GenerateNewId().ToString();
            var user2Id = ObjectId.GenerateNewId().ToString();

            var tracks = new List<Track> { new Track { UserId = user1Id, Title = "example 1" }, new Track { UserId = user2Id, Title = "example 2" } };
            _mockMongoDbService.Setup(service => service.GetTracksByUserAsync(user1Id)).ReturnsAsync(tracks);

            // Act
            var result = await _trackService.GetTracksByUser(user1Id);

            // Assert
            Xunit.Assert.Equal(tracks, result);
        }

        [Fact]
        public async Task DeleteTrack_Success_ReturnsTrue()
        {
            // Arrange
            var trackId = ObjectId.GenerateNewId().ToString();
            _mockMongoDbService.Setup(service => service.DeleteTrackAsync(trackId)).ReturnsAsync(true);

            // Act
            var result = await _trackService.DeleteTrack(trackId);

            // Assert
            Xunit.Assert.True(result);
        }

        [Fact]
        public async Task DeleteTrack_Failure_ReturnsFalse()
        {
            // Arrange
            var trackId = ObjectId.GenerateNewId().ToString();
            _mockMongoDbService.Setup(service => service.DeleteTrackAsync(trackId)).ReturnsAsync(false);

            // Act
            var result = await _trackService.DeleteTrack(trackId);

            // Assert
            Xunit.Assert.False(result);
        }
    }
}