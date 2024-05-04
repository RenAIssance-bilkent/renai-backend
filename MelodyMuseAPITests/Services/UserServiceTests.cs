using Microsoft.VisualStudio.TestTools.UnitTesting;
using MelodyMuseAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::MelodyMuseAPI.Dtos;
using Moq;
using Xunit;
using global::MelodyMuseAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;


namespace MelodyMuseAPI.Services.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<MongoDbService> _mockMongoDbService;

        [TestInitialize]
        public void Setup()
        {
            _mockMongoDbService = new Mock<MongoDbService>();
            _userService = new UserService(_mockMongoDbService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange

            var users = new List<UserDto> { new UserDto { Id = ObjectId.GenerateNewId().ToString(), Name = "User1", Email = "user1@example.com" }, new UserDto { Id = ObjectId.GenerateNewId().ToString(), Name = "User2", Email = "user2@example.com" } };
            _mockMongoDbService.Setup(service => service.GetAllAsync()).ReturnsAsync(users);
            var userService = new UserService(_mockMongoDbService.Object);

            // Act
            var result = await userService.GetAllUsers();

            // Assert
            Assert.Equals(users, result);
        }

        [Fact]
        public async Task GetUserByEmailTest_ReturnsUser()
        {

            var email = "user@example.com";
            var fakeUser = new UserDto { Email = email };

            _mockMongoDbService.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync(fakeUser);

            var result = await _userService.GetUserByEmail(email);

            var userService = new UserService(_mockMongoDbService.Object);


            // Assert
            Assert.Equals(email, result.Email);

            Assert.IsNotNull(result);
        }

        [Fact]
        public async Task ChangePasswordTest_SuccessfulChange()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var currentPassword = "oldPassword";
            var newPassword = "newPassword";
            var userChangePasswordDto = new UserChangePasswordDto { CurrentPassword = currentPassword, NewPassword = newPassword };
            _mockMongoDbService.Setup(service => service.ValidatePasswordHashByIdAsync(userId, currentPassword)).ReturnsAsync(true);
            _mockMongoDbService.Setup(service => service.UpdateUserPasswordAsync(userId, It.IsAny<string>())).ReturnsAsync(true);
            var userService = new UserService(_mockMongoDbService.Object);

            // Act
            var result = await userService.ChangePassword(userId, userChangePasswordDto);

            // Assert
            Assert.IsTrue(result);
        }

        [Fact]
        public async Task DeleteUserTest_UserDeleted()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var deleteResult = new DeleteResult.Acknowledged(1);
            _mockMongoDbService.Setup(service => service.DeleteUserAsync(userId)).ReturnsAsync(deleteResult);
            var userService = new UserService(_mockMongoDbService.Object);

            // Act
            var result = await userService.DeleteUser(userId);

            // Assert
            Assert.IsTrue(result);
        }

    }

}