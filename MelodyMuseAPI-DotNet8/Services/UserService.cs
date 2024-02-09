using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Data;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MelodyMuseAPI_DotNet8.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDBService _mongoDBService;

        public UserService(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _mongoDBService.GetAllAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _mongoDBService.GetUserByEmailAsync(email);
        }
        public async Task<User> GetUserById(string userId)
        {
            return await _mongoDBService.GetUserByIdAsync(userId);
        }
        public async Task<User> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            var existingUser = await _mongoDBService.GetUserByEmailAsync(userRegistrationDto.Email);
            if (existingUser != null)
            {
                //TODO: change it to error handling
                throw new InvalidOperationException("User already exists with the provided email address.");
            }

            var newUser = new User
            {
                Name = userRegistrationDto.Username,
                Email = userRegistrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                TrackIds = new List<string>(),
                Points = 0
            };

            await _mongoDBService.AddUserAsync(newUser);
            return newUser;
        }
        public Task<UserTokenDto> AuthenticateUser(UserLoginDto userLoginDto)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto)
        {
            var user = await _mongoDBService.GetUserByIdAsync(userId);
            if (user != null && BCrypt.Net.BCrypt.Verify(userChangePasswordDto.CurrentPassword, user.PasswordHash))
            {
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(userChangePasswordDto.NewPassword);
                return await _mongoDBService.UpdateUserPasswordAsync(userId, newPasswordHash);
            }

            return false;
        }
        public async Task<bool> DeleteUser(string userId)
        {
            var result = await _mongoDBService.DeleteUserAsync(userId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        public Task<bool> PurchasePoints(string userId, int points)
        {
            //TODO: add Stripe API connection
            throw new NotImplementedException();
        }

        public Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            //TODO: implement after acquiring corporate email
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateUser(string userId, UserProfileUpdateDto userProfileUpdateDto)
        {
            var updateDefinition = Builders<User>.Update
                .Set(u => u.Name, userProfileUpdateDto.Name)
                .Set(u => u.Email, userProfileUpdateDto.Email);
            //TODO: add profile picture

            return await _mongoDBService.UpdateUserAsync(userId, updateDefinition);
        }
    }
}
