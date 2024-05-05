using Xunit;
using Moq;
using MelodyMuseAPI.Controllers;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MelodyMuseAPI.Dtos;
using MongoDB.Bson;

namespace MelodyMuseAPI.Controllers.Tests
{
    public class TrackControllerTests
    {
        private readonly TrackController _trackController;
        private readonly Mock<ITrackService> _mockTrackService;
        private readonly Mock<OpenAIApiService> _mockOpenAIApiService;

        public TrackControllerTests()
        {
            _mockTrackService = new Mock<ITrackService>();
            _mockOpenAIApiService = new Mock<OpenAIApiService>();
            _trackController = new TrackController(_mockTrackService.Object, _mockOpenAIApiService.Object);
        }

        [Fact]
        public async Task GenerateTrack_WithValidMetadata_ReturnsTrackId()
        {
            // Arrange
            var metadata = new Metadata();
            var userId = ObjectId.GenerateNewId().ToString();
            var expectedTrackId = ObjectId.GenerateNewId().ToString();

            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var userClaimsIdentity = new ClaimsIdentity(userClaims);
            var userClaimsPrincipal = new ClaimsPrincipal(userClaimsIdentity);
            _trackController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaimsPrincipal }
            };

            _mockTrackService.Setup(service => service.GenerateTrack(metadata, userId)).ReturnsAsync(expectedTrackId);

            // Act
            var result = await _trackController.GenerateTrack(metadata);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTrackId, okObjectResult.Value);
        }

        [Fact]
        public async Task GenerateTrack_WithEmptyUserId_ReturnsUnauthorized()
        {
            // Arrange
            var metadata = new Metadata();
            _trackController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _trackController.GenerateTrack(metadata);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Unauthorized Access.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GenerateMetadata_WithValidTrackCreationDto_ReturnsOkResult()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto();
            var userId = ObjectId.GenerateNewId().ToString();
            var expectedMetadata = new Metadata();

            var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var userClaimsIdentity = new ClaimsIdentity(userClaims);
            var userClaimsPrincipal = new ClaimsPrincipal(userClaimsIdentity);
            _trackController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaimsPrincipal }
            };

            _mockTrackService.Setup(service => service.GenerateTrackMetadata(trackCreationDto, userId)).ReturnsAsync(expectedMetadata);

            // Act
            var result = await _trackController.GenerateMetadata(trackCreationDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedMetadata, okObjectResult.Value);
        }

        [Fact]
        public async Task GenerateMetadata_WithEmptyUserId_ReturnsUnauthorized()
        {
            // Arrange
            var trackCreationDto = new TrackCreationDto();
            _trackController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _trackController.GenerateMetadata(trackCreationDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Unauthorized Access.", unauthorizedResult.Value);
        }

    }
}
