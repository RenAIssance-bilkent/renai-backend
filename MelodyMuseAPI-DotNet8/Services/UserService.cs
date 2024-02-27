using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Data;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using System.Security.Claims;
using System.Text;


namespace MelodyMuseAPI_DotNet8.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDbService _mongoDbService;

        public UserService(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _mongoDbService.GetAllAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _mongoDbService.GetUserByEmailAsync(email);
        }

        public async Task<User> GetUserById(string userId)
        {
            return await _mongoDbService.GetUserByIdAsync(userId);
        }

        public async Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto)
        {
            var user = await _mongoDbService.GetUserByIdAsync(userId);
            if (user != null && BCrypt.Net.BCrypt.Verify(userChangePasswordDto.CurrentPassword, user.PasswordHash))
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
            //TODO: add Stripe API connection
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUser(string userId, UserProfileUpdateDto userProfileUpdateDto)
        {
            var updateDefinition = Builders<User>.Update
                .Set(u => u.Name, userProfileUpdateDto.Name)
                .Set(u => u.Email, userProfileUpdateDto.Email);
            //TODO: add profile picture

            return await _mongoDbService.UpdateUserAsync(userId, updateDefinition);
        }
    }
}
