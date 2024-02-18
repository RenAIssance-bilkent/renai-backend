using Amazon.SecurityToken.Model;
using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using MelodyMuseAPI_DotNet8.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MelodyMuseAPI_DotNet8.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var userToken = await _authService.LoginUser(userLoginDto);
                return Ok(userToken);
            }
            catch (InvalidOperationException ex)
            {
                // TODO: handle the thrown exception for wrong credentials if needed
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            try
            {
                var newUser = await _authService.RegisterUser(userRegistrationDto);
                return Ok(newUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
