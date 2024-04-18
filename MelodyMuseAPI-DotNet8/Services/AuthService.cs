using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Settings;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace MelodyMuseAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly EmailSenderService _emailSenderService;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AuthService(MongoDbService mongoDbService, EmailSenderService emailSenderService, IOptions<JwtSettings> jwtSettings)
        {
            _mongoDbService = mongoDbService;
            _emailSenderService = emailSenderService;
            _jwtSettings = jwtSettings;
        }

        public async Task<LoginResult> LoginUser(UserLoginDto userLoginDto)
        {
            var isValid = await _mongoDbService.ValidatePasswordHashByEmailAsync(userLoginDto.Email, userLoginDto.Password);

            if (!isValid)
            {
                return new LoginResult { Success = false, Errors = new List<string> { "Invalid credentials provided." } };
            }

            var existingUser = await _mongoDbService.GetUserByEmailAsync(userLoginDto.Email);

            var userTokenDto = new UserTokenDto
            {
                Token = CreateToken(existingUser),
                ExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationMinutes),
                Id = existingUser.Id,
                Email = existingUser.Email,
                Name = existingUser.Name,
            };

            return new LoginResult { Success = true, User = userTokenDto };
        }

        public async Task<RegistrationResult> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            var existingUser = await _mongoDbService.GetUserByEmailAsync(userRegistrationDto.Email);
            if (existingUser != null)
            {
                return new RegistrationResult { Success = false, Errors = new List<string> { "Email is already in use." } };
            }

            var confirmationToken = GenerateConfirmationToken();

            var newUser = new User
            {
                Name = userRegistrationDto.Name,
                Email = userRegistrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                ConfirmationToken = confirmationToken, // Stored token in user record
                IsConfirmed = false,
                TrackIds = new List<string>(),
                Points = 0
            };

           _emailSenderService.SendConfirmEmailAsync(userRegistrationDto.Name, userRegistrationDto.Email, confirmationToken);

           await _mongoDbService.AddUserAsync(newUser);

           return new RegistrationResult { Success = true };
        }

        public async Task<bool> ConfirmEmailAsync(string token, string email)
        {
            var result = await _mongoDbService.ValidateConfirmationTokenAsync(email, token);

            return result;
        }

        public Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            throw new NotImplementedException("Reset password feature is not implemented yet.");
        }

        private string CreateToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds,
                Issuer = _jwtSettings.Value.Issuer,
                Audience = _jwtSettings.Value.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateConfirmationToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("action", "email_confirmation")
                }),
                Expires = expires,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public UserTokenDto User { get; set; }
        public List<string> Errors { get; set; }

        public LoginResult()
        {
            Errors = new List<string>();
        }
    }
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }

        public RegistrationResult()
        {
            Errors = new List<string>();
        }
    }


}
