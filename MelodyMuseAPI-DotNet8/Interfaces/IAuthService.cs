using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;

namespace MelodyMuseAPI_DotNet8.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterUser(UserRegistrationDto userRegistrationDto);
        Task<UserTokenDto> LoginUser(UserLoginDto userLoginDto);
        Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto);
    }
}
