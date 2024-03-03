using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;

namespace MelodyMuseAPI.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterUser(UserRegistrationDto userRegistrationDto);
        Task<UserTokenDto> LoginUser(UserLoginDto userLoginDto);
        Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto);
    }
}
