using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;

namespace MelodyMuseAPI.Interfaces
{
    public interface IUserService
    {
        //TODO: this mehtod is ONLY for testing, delete of release
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(string userId);
        Task<UserDto> GetUserByEmail(string email);
        Task<bool> UpdateUser(string userId, UserProfileUpdateDto userProfileUpdateDto);
        Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto);
        Task<bool> DeleteUser(string userId);
        Task<bool> PurchasePoints(string userId, int points);

    }
}
