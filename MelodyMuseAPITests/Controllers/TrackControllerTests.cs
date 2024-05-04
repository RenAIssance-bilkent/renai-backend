using Xunit;
using Moq;
using MelodyMuseAPI.Controllers;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace MelodyMuseAPI.Tests.Controllers
{
    public class TrackControllerTests
    {
        private readonly TrackController _controller;
        private readonly Mock<ITrackService> _mockTrackService;

        public TrackControllerTests()
        {
            _mockTrackService = new Mock<ITrackService>();
            _controller = new TrackController(_mockTrackService.Object);
        }

        [Fact]
        public async Task GenerateTrack_ReturnsUnauthorized_WhenUserIdIsNullOrEmpty()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.GenerateTrack(trackCreationDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Unauthorized Access.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetTrackById_ReturnsOkResult_WhenTrackExists()
        {
            // Arrange
            var trackId = ObjectId.GenerateNewId().ToString();
            var track = new TrackRetrivalDto { Id = trackId };
            _mockTrackService.Setup(service => service.GetTrackById(trackId)).ReturnsAsync(track);

            // Act
            var result = await _controller.GetTrackById(trackId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var retrievedTrack = Assert.IsType<TrackRetrivalDto>(okResult.Value);
            Assert.Equal(trackId, retrievedTrack.Id);
        }

      
        [Fact]
        public async Task GetAllTracks_ReturnsOkResult_WhenTracksExist()
        {
            // Arrange
            var tracks = new List<TrackRetrivalDto> { new TrackRetrivalDto(), new TrackRetrivalDto() };
            _mockTrackService.Setup(service => service.GetAllTracks()).ReturnsAsync(tracks);

            // Act
            var result = await _controller.GetAllTracks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var retrievedTracks = Assert.IsAssignableFrom<IEnumerable<TrackRetrivalDto>>(okResult.Value);
            Assert.Equal(2, retrievedTracks.Count);
        }

        [Fact]
        public async Task DeleteTrack_ReturnsNotFound_WhenTrackDoesNotExist()
        {
            // Arrange
            var trackId = ObjectId.GenerateNewId().ToString();
            _mockTrackService.Setup(service => service.DeleteTrack(trackId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTrack(trackId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Track with ID {trackId} not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task SearchTracks_ReturnsNotFound_WhenNoTracksMatchTitle()
        {
            // Arrange
            var title = "NonExistingTitle";
            _mockTrackService.Setup(service => service.SearchTracks(title)).ReturnsAsync(new List<TrackRetrivalDto>());

            // Act
            var result = await _controller.SearchTracks(title);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"No tracks found matching '{title}'.", notFoundResult.Value);
        }
    }
}
