using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MelodyMuseAPI.Services
{
    public class AuthService : IAuthService
    {

        private readonly MongoDbService _mongoDbService;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AuthService(MongoDbService mongoDbService, IOptions<JwtSettings> jWTSettings)
        {
            _mongoDbService = mongoDbService;
            _jwtSettings = jWTSettings;
        }

        public async Task<UserTokenDto> LoginUser(UserLoginDto userLoginDto)
        {

            var existingUser = await _mongoDbService.GetUserByEmailAsync(userLoginDto.Email);
            if (existingUser is null)
            {
                throw new InvalidOperationException("Wrong Credentials!");
            }
            bool isValidPasswd = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, existingUser.PasswordHash);

            if (!isValidPasswd)
            {
                throw new InvalidOperationException("Wrong Credentials!");
            }

            return CreateToken(existingUser);
        }

        public async Task<User> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            var existingUser = await _mongoDbService.GetUserByEmailAsync(userRegistrationDto.Email);
            if (existingUser != null)
            {
                //TODO: change it to error handling
                throw new InvalidOperationException("User already exists with the provided email address.");
            }

            var newUser = new User
            {
                Name = userRegistrationDto.Username,
                Email = userRegistrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password),
                TrackIds = new List<string>(),
                Points = 0
            };

            await _mongoDbService.AddUserAsync(newUser);
            return newUser;
        }

        public Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            //TODO: implement after acquiring corporate email
            throw new NotImplementedException();
        }

        private UserTokenDto CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                // TODO: ask if you need to add Id to Claim
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };


            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires : expires,
                signingCredentials : cred,
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            UserTokenDto userTokenDto = new UserTokenDto()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ExpiryDate = expires,
                Token = jwt
            };

            return userTokenDto;
        }
    }
}
