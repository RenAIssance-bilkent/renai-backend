using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;

namespace MelodyMuseAPI_DotNet8.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUser(UserRegistrationDto userRegistrationDto);
        Task<UserTokenDto> AuthenticateUser(UserLoginDto userLoginDto);
        Task<bool> UpdateUserProfile(string userId, UserProfileUpdateDto userProfileUpdateDto);
        Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto);
        Task<User> GetUserById(string userId);
        Task<bool> DeleteUser(string userId);
        Task<User> GetUserByEmail(string email);
        Task<bool> PurchasePoints(string userId, int points);
        Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto);


    }
}
