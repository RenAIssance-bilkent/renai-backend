using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Data;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MelodyMuseAPI_DotNet8.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDBService _mongoDBService;


        //Note for Nejo
        //did not initialize ???
        private IOptions<JWTSettings> _jwtSettings;


        public UserService(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _mongoDBService.GetAllAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _mongoDBService.GetUserByEmailAsync(email);
        }
        public async Task<User> GetUserById(string userId)
        {
            return await _mongoDBService.GetUserByIdAsync(userId);
        }
        public async Task<User> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            var existingUser = await _mongoDBService.GetUserByEmailAsync(userRegistrationDto.Email);
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

            await _mongoDBService.AddUserAsync(newUser);
            return newUser;
        }
        
        public async Task<UserTokenDto> AuthenticateUser(UserLoginDto userLoginDto)
        {

            var existingUser = await _mongoDBService.GetUserByEmailAsync(userLoginDto.Email);
            if (existingUser is null)
            {
                //return null;
                throw new InvalidOperationException("Wrong Credentials!"); // Exception will be thrown
            }
            bool isValidPasswd = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, existingUser.PasswordHash);
            
            if (!isValidPasswd)
            {
                //return null;
                throw new InvalidOperationException("Wrong Credentials!"); // Exception will be thrown
            }

            // Password is correct, generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.SecretKey); // Assuming _jwtSettings contains the secret key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id),
                new Claim(ClaimTypes.Email, existingUser.Email)
                // Add more claims as needed
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationMinutes), // Set expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return a DTO with user information and token
            var userTokenDto = new UserTokenDto
            {
                UserId = existingUser.Id,
                Token = tokenHandler.WriteToken(token),
                ExpiryDate = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpirationMinutes), // Ensure ExpiryDate is set
                Username = existingUser.Name
                // Add more user information as needed
            };

            return userTokenDto;


            //throw new NotImplementedException();
        }

        //public string GenerateToken(string userId, string userEmail)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(secretKey);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, userId),
        //            new Claim(ClaimTypes.Email, userEmail)
        //        }),
        //        Expires = DateTime.UtcNow.AddMinutes(expirationMinutes), // Set expiration time
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}


        public async Task<bool> ChangePassword(string userId, UserChangePasswordDto userChangePasswordDto)
        {
            var user = await _mongoDBService.GetUserByIdAsync(userId);
            if (user != null && BCrypt.Net.BCrypt.Verify(userChangePasswordDto.CurrentPassword, user.PasswordHash))
            {
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(userChangePasswordDto.NewPassword);
                return await _mongoDBService.UpdateUserPasswordAsync(userId, newPasswordHash);
            }

            return false;
        }
        public async Task<bool> DeleteUser(string userId)
        {
            var result = await _mongoDBService.DeleteUserAsync(userId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        public Task<bool> PurchasePoints(string userId, int points)
        {
            //TODO: add Stripe API connection
            throw new NotImplementedException();
        }

        public Task<bool> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            //TODO: implement after acquiring corporate email
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateUser(string userId, UserProfileUpdateDto userProfileUpdateDto)
        {
            var updateDefinition = Builders<User>.Update
                .Set(u => u.Name, userProfileUpdateDto.Name)
                .Set(u => u.Email, userProfileUpdateDto.Email);
            //TODO: add profile picture

            return await _mongoDBService.UpdateUserAsync(userId, updateDefinition);
        }
    }
}
