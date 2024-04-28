using MelodyMuseAPI.Models;
using MelodyMuseAPI.Data;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using System.Security.Claims;
using System.Text;


namespace MelodyMuseAPI.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDbService _mongoDbService;

        public UserService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            return await _mongoDbService.GetAllAsync();
        }

        public async Task<UserDto> GetUserByEmail(string email)
        {
            return await _mongoDbService.GetUserByEmailAsync(email);
        }

        public async Task<UserDto> GetUserById(string userId)
        {
            return await _mongoDbService.GetUserByIdAsync(userId);
        }

        public async Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto)
        {
            var isValid = await _mongoDbService.ValidatePasswordHashByIdAsync(userId, userChangePasswordDto.CurrentPassword);

            if (isValid)
            {
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(userChangePasswordDto.NewPassword);
                return await _mongoDbService.UpdateUserPasswordAsync(userId, newPasswordHash);
            }

            return false;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            var result = await _mongoDbService.DeleteUserAsync(userId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public Task<bool> PurchasePoints(string userId, int points)
        {
            if (points < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(points), "Points to add cannot be negative.");
            }
            return _mongoDbService.AddUserPoints(userId, points);
        }

        public async Task<bool> UpdateUser(string userId, UserProfileUpdateDto userProfileUpdateDto)
        {
            var existingUser = await _mongoDbService.GetUserByEmailAsync(userProfileUpdateDto.Email);
            if (existingUser != null && existingUser.Id != userId)
            {
                return false;
            }
            var isValid = await _mongoDbService.ValidatePasswordHashByEmailAsync(userId, userProfileUpdateDto.Password);

            if (!isValid)
            {
                return false;
            }

            var updateDefinition = Builders<User>.Update
                .Set(u => u.Name, userProfileUpdateDto.Name)
                .Set(u => u.Email, userProfileUpdateDto.Email);

            return await _mongoDbService.UpdateUserAsync(userId, updateDefinition);
        }

    }
}
