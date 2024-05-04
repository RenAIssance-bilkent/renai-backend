using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Services;
using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

#nullable disable


namespace MelodyMuseAPITests.Services
{
    public class AuthServiceTests
    {
        private AuthService _authService;
        private Mock<MongoDbService> _mockMongoDbService;
        private Mock<EmailSenderService> _mockEmailSenderService;
        private Mock<IOptions<JwtSettings>> _mockJwtSettings;

        public AuthServiceTests()
        {
            _mockMongoDbService = new Mock<MongoDbService>();
            _mockEmailSenderService = new Mock<EmailSenderService>();
            _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
            _authService = new AuthService(_mockMongoDbService.Object, _mockEmailSenderService.Object, _mockJwtSettings.Object);
        }

        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { Email = "user@example.com", Password = "password" };
            var userDto = new UserDto { Email = "user@example.com", Name = "User", Id = "123" };
            _mockMongoDbService.Setup(service => service.ValidatePasswordHashByEmailAsync(userLoginDto.Email, userLoginDto.Password)).ReturnsAsync(true);
            _mockMongoDbService.Setup(service => service.GetUserByEmailAsync(userLoginDto.Email)).ReturnsAsync(userDto);
            _mockJwtSettings.Setup(x => x.Value).Returns(new JwtSettings { ExpirationMinutes = 30, Issuer = "issuer", Audience = "audience", SecretKey = "supersecretkey" });

            // Act
            var result = await _authService.LoginUser(userLoginDto);

            // Assert
            Xunit.Assert.True(result.Success);
            Xunit.Assert.NotNull(result.User);
            Xunit.Assert.Equal(userDto.Email, result.User.Email);
        }

        [Fact]
        public async Task RegisterUser_NewUser_ReturnsSuccess()
        {
            // Arrange
            var userRegistrationDto = new UserRegistrationDto { Email = "newuser@example.com", Name = "New User", Password = "password" };
            var jwtSettings = new JwtSettings { ExpirationMinutes = 30, Issuer = "issuer", Audience = "audience", SecretKey = "supersecretkey" };
            _mockJwtSettings.Setup(x => x.Value).Returns(jwtSettings);
            _mockMongoDbService.Setup(service => service.GetUserByEmailAsync(userRegistrationDto.Email)).ReturnsAsync((UserDto)null);
            _mockMongoDbService.Setup(service => service.AddUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUser(userRegistrationDto);

            // Assert
            Xunit.Assert.True(result.Success);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ValidToken_ReturnsTrue()
        {
            // Arrange
            var token = "validtoken";
            var email = "user@example.com";
            _mockMongoDbService.Setup(service => service.ValidateConfirmationTokenAsync(email, token)).ReturnsAsync(true);

            // Act
            var result = await _authService.ConfirmEmailAsync(token, email);

            // Assert
            Xunit.Assert.True(result);
        }

        [Fact]
        public async Task ConfirmEmailAsync_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var token = "invalidtoken";
            var email = "user@example.com";
            _mockMongoDbService.Setup(service => service.ValidateConfirmationTokenAsync(email, token)).ReturnsAsync(false);

            // Act
            var result = await _authService.ConfirmEmailAsync(token, email);

            // Assert
            Xunit.Assert.False(result);
        }
    }
}
