using Xunit;
using Moq;
using MelodyMuseAPI.Services;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Reflection;

namespace MelodyMuseAPI.Tests
{
    public class TrackServiceTests
    {
        private TrackService _trackService;
        private Mock<MongoDbService> _mockMongoDbService;
        private Mock<OpenAIApiService> _mockOpenAIApiService;
        private Mock<ModelService> _mockModelService;

        public TrackServiceTests()
        {
            _mockMongoDbService = new Mock<MongoDbService>();
            _mockOpenAIApiService = new Mock<OpenAIApiService>();
            _mockModelService = new Mock<ModelService>();

            _trackService = new TrackService(_mockMongoDbService.Object, _mockOpenAIApiService.Object, _mockModelService.Object);
        }

        [Fact]
        public async Task GenerateTrackMetadata_PromptProvided_ReturnsMetadata()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto { Prompt = "Test Prompt" };
            var userId = ObjectId.GenerateNewId().ToString();
            var expectedMetadata = new Metadata { Title="deneme" };

            _mockOpenAIApiService.Setup(x => x.GetMetadataFromPromptForReplica(trackCreationDto, userId)).ReturnsAsync(expectedMetadata);

            // Act
            var result = await _trackService.GenerateTrackMetadata(trackCreationDto, userId);

            // Assert
            Assert.Equal(expectedMetadata, result);
        }

        [Fact]
        public async Task GenerateTrackMetadata_GenreProvided_GeneratesPrompt()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto { Genre = "Test Genre" };
            var userId = ObjectId.GenerateNewId().ToString();
            var generatedPrompt = "Generated Prompt";

            _mockOpenAIApiService.Setup(x => x.GeneratePromptBasedOnGenre(trackCreationDto.Genre, userId)).ReturnsAsync(generatedPrompt);

            // Act
            await _trackService.GenerateTrackMetadata(trackCreationDto, userId);

            // Assert
            _mockOpenAIApiService.Verify(x => x.GeneratePromptBasedOnGenre(trackCreationDto.Genre, userId), Times.Once);
        }

        // Add more tests for other methods

        [Fact]
        public async Task GetAllTracks_ReturnsAllTracks()
        {
            // Arrange
            var expectedTracks = new List<Track> { new Track { Title="kamran"}, new Track { Title="nejo"} };
            _mockMongoDbService.Setup(x => x.GetAllTracksAsync()).ReturnsAsync(expectedTracks);

            // Act
            var result = await _trackService.GetAllTracks();

            // Assert
            Assert.Equal(expectedTracks, result);
        }

    }
}
