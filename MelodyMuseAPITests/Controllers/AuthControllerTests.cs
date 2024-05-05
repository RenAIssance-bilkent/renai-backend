using Xunit;
using Moq;
using MelodyMuseAPI.Controllers;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MelodyMuseAPI.Services;

#nullable disable

namespace MelodyMuseAPI.Controllers.Tests
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IAuthService> _mockAuthService;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Login_WithValidUserLoginDto_ReturnsOkResult()
        {
            // Arrange
            var userLoginDto = new UserLoginDto();
            var userTokenDto = new UserTokenDto { Token = "sampleToken", ExpiryDate = DateTime.Now.AddDays(1), Id = "userId", Email = "test@example.com", Name = "Test User" };
            _mockAuthService.Setup(service => service.LoginUser(userLoginDto)).ReturnsAsync(new LoginResult { Success = true, User = userTokenDto });

            // Act
            var result = await _authController.Login(userLoginDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userTokenDto, okObjectResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidUserLoginDto_ReturnsUnauthorized()
        {
            // Arrange
            var userLoginDto = new UserLoginDto();
            _mockAuthService.Setup(service => service.LoginUser(userLoginDto)).ReturnsAsync((LoginResult)null);

            // Act
            var result = await _authController.Login(userLoginDto);

            // Assert
            var unauthorizedObjectResult = Assert.IsType<UnauthorizedResult>(result);
            Assert.Equal(401, unauthorizedObjectResult.StatusCode);
        }

        [Fact]
        public async Task Register_WithValidUserRegistrationDto_ReturnsOkResult()
        {
            // Arrange
            var userRegistrationDto = new UserRegistrationDto();
            var userDto = new UserDto { Id = "userId", Email = "test@example.com", Name = "Test User", Points = 0 };
            _mockAuthService.Setup(service => service.RegisterUser(userRegistrationDto)).ReturnsAsync(new RegistrationResult { Success = true });

            // Act
            var result = await _authController.Register(userRegistrationDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userDto, okObjectResult.Value);
        }

        [Fact]
        public async Task Register_WithInvalidUserRegistrationDto_ReturnsBadRequest()
        {
            // Arrange
            var userRegistrationDto = new UserRegistrationDto();
            _mockAuthService.Setup(service => service.RegisterUser(userRegistrationDto)).ReturnsAsync((RegistrationResult)null);

            // Act
            var result = await _authController.Register(userRegistrationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task ConfirmEmail_WithCorrectTokenAndEmail_ReturnsOkResult()
        {
            // Arrange
            var token = "sampleToken";
            var email = "test@example.com";
            _mockAuthService.Setup(service => service.ConfirmEmailAsync(token, email)).ReturnsAsync(true);

            // Act
            var result = await _authController.ConfirmEmail(token, email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email confirmed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_WithIncorrectTokenOrEmail_ReturnsBadRequest()
        {
            // Arrange
            var token = "invalidToken";
            var email = "test@example.com";
            _mockAuthService.Setup(service => service.ConfirmEmailAsync(token, email)).ReturnsAsync(false);

            // Act
            var result = await _authController.ConfirmEmail(token, email);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Incorrect token or identification.", badRequestResult.Value);
        }
    }
}
