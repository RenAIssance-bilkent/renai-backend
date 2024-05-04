using Xunit;
using Moq;
using MelodyMuseAPI.Controllers;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using MelodyMuseAPI.Services;
using MongoDB.Bson;

namespace MelodyMuseAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<EmailSenderService> _mockEmailSenderService;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockEmailSenderService = new Mock<EmailSenderService>();
            _controller = new UserController(_mockUserService.Object, _mockEmailSenderService.Object);
        }

        [Fact]
        public async Task GetUserById_ReturnsUnauthorized_WhenUserIdsDoNotMatch()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var userIdClaim = new Claim(ClaimTypes.NameIdentifier, "differentId");
            var userClaims = new List<Claim> { userIdClaim };
            var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "TestAuthentication"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetUserById(id);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Unauthorized Access.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsUnauthorized_WhenUserIdsDoNotMatch()
        {
            // Arrange
            var userId = ObjectId.GenerateNewId().ToString();
            var differentId = ObjectId.GenerateNewId().ToString();
            var userIdClaim = new Claim(ClaimTypes.NameIdentifier, differentId);
            var userClaims = new List<Claim> { userIdClaim };
            var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "TestAuthentication"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            var updateDto = new UserProfileUpdateDto();

            // Act
            var result = await _controller.UpdateUser(userId, updateDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Unauthorized Access.", unauthorizedResult.Value);
        }

        
        [Fact]
        public async Task PurchasePoints_ReturnsOkResult_WhenPurchaseIsSuccessful()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            // Arrange
            var ppDto = new PurchasePointsDto { UserId = userId, Points = 100 };
            var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userId);
            var userClaims = new List<Claim> { userIdClaim };
            var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "TestAuthentication"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            _mockUserService.Setup(service => service.PurchasePoints(ppDto.UserId, ppDto.Points))
                            .ReturnsAsync(true);

            // Act
            var result = await _controller.PurchasePoints(ppDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }
    }
}
