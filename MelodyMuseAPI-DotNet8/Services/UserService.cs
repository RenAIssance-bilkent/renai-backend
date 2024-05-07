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
        private readonly EmailSenderService _emailSenderService;

        public UserService(MongoDbService mongoDbService, EmailSenderService emailSenderService)
        {
            _mongoDbService = mongoDbService;
            _emailSenderService = emailSenderService;
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

            if (isValid && userChangePasswordDto.NewPassword == userChangePasswordDto.ConfirmNewPassword)
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
            var existingUser = await _mongoDbService.GetUserByIdAsync(userId);
            if (existingUser == null)
            {
                return false;
            }

            // Validate the user's password to authorize changes
            var isValid = await _mongoDbService.ValidatePasswordHashByIdAsync(userId, userProfileUpdateDto.Password);
            if (!isValid)
            {
                return false;
            }

            // Define the update builder outside of conditionals to accumulate changes
            var updateBuilder = Builders<User>.Update;

            // Initially set the update definition to null
            UpdateDefinition<User> updateDefinition = null;

            // If the email has changed, prepare email update and token generation
            if (existingUser.Email != userProfileUpdateDto.Email)
            {
                var token = _emailSenderService.GenerateConfirmationToken();

                // Start building the update definition for email change
                updateDefinition = updateBuilder.Set(u => u.Email, userProfileUpdateDto.Email)
                                                .Set(u => u.IsConfirmed, false)
                                                .Set(u => u.ConfirmationToken, token);

                // Send email to confirm new email
                _emailSenderService.SendEditConfirmationEmail(userProfileUpdateDto.Name, userProfileUpdateDto.Email, token);
            }

            // If the name is different and needs to be updated
            if (existingUser.Name != userProfileUpdateDto.Name)
            {
                if (updateDefinition == null)
                {
                    // If no update definition yet, start a new one
                    updateDefinition = updateBuilder.Set(u => u.Name, userProfileUpdateDto.Name);
                }
                else
                {
                    // Otherwise, combine it with the existing one
                    updateDefinition = updateDefinition.Set(u => u.Name, userProfileUpdateDto.Name);
                }
            }

            // If there were changes, apply the update
            if (updateDefinition != null)
            {
                return await _mongoDbService.UpdateUserAsync(userId, updateDefinition);
            }

            // If no fields were changed, return true as the update is "successful" in the sense that nothing needed changing
            return true;
        }

    }
}
