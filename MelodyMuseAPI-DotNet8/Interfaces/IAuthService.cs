using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Services;

namespace MelodyMuseAPI.Interfaces
{
    public interface IAuthService
    {
        Task<RegistrationResult> RegisterUser(UserRegistrationDto userRegistrationDto);
        Task<LoginResult> LoginUser(UserLoginDto userLoginDto);
        Task<bool> ConfirmEmailAsync(string token, string email);
        Task<bool> ResetPassword(string email);
    }
}
