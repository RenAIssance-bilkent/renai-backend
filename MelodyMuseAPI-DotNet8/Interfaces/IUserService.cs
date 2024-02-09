using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;

namespace MelodyMuseAPI_DotNet8.Interfaces
{
    public interface IUserService
    {
        //TODO: this mehtod is ONLY for testing, delete of release
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(string userId);
        Task<User> GetUserByEmail(string email);
        Task<User> RegisterUser(UserRegistrationDto userRegistrationDto);
        Task<UserTokenDto> AuthenticateUser(UserLoginDto userLoginDto);
        Task<bool> UpdateUser(string userId, UserProfileUpdateDto userProfileUpdateDto);
        Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto);
        Task<bool> DeleteUser(string userId);
        Task<bool> PurchasePoints(string userId, int points);
        Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto);


    }
}
