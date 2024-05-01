using Microsoft.VisualStudio.TestTools.UnitTesting;
using MelodyMuseAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelodyMuseAPI.Services.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
using MelodyMuseAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using global::MelodyMuseAPI.Dtos;
    using Moq;

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

            [TestMethod]
            public async Task GetAllUsersTest_ReturnsAllUsers()
            {
                var fakeUsers = new List<UserDto> { new UserDto { Email = "user@example.com" } };
                _mockMongoDbService.Setup(s => s.GetAllAsync()).ReturnsAsync(fakeUsers);

                var users = await _userService.GetAllUsers();

                Assert.AreEqual(1, users.Count);
                Assert.AreEqual("user@example.com", users[0].Email);
            }

            [TestMethod]
            public async Task GetUserByEmailTest_ReturnsUser()
            {

                var email = "user@example.com";
                var fakeUser = new UserDto { Email = email };
                _mockMongoDbService.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync(fakeUser);

                var result = await _userService.GetUserByEmail(email);

                Assert.IsNotNull(result);
                Assert.AreEqual(email, result.Email);
            }

            [TestMethod]
            public async Task ChangePasswordTest_SuccessfulChange()
            {
                var userId = "123";
                var currentPassword = "oldPass";
                var newPassword = "newPass";
                var changePasswordDto = new UserChangePasswordDto { CurrentPassword = currentPassword, NewPassword = newPassword };

                _mockMongoDbService.Setup(s => s.ValidatePasswordHashByIdAsync(userId, currentPassword)).ReturnsAsync(true);
                _mockMongoDbService.Setup(s => s.UpdateUserPasswordAsync(userId, It.IsAny<string>())).ReturnsAsync(true);

                var result = await _userService.ChangePassword(userId, changePasswordDto);

                Assert.IsTrue(result);
            }

            [TestMethod]
            public async Task DeleteUserTest_UserDeleted()
            {
                var userId = "123";
                var deleteResult = new MongoDB.Driver.DeleteResult.Acknowledged(1);
                _mockMongoDbService.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(deleteResult);

                var result = await _userService.DeleteUser(userId);
                Assert.IsTrue(result);
            }

        }
    }
}