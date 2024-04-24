using Microsoft.AspNetCore.Mvc;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace MelodyMuseAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableCors("AllowWebApp")]
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
            var loginResult = await _authService.LoginUser(userLoginDto);
            if (!loginResult.Success)
            {
                return Unauthorized(new { Errors = loginResult.Errors });
            }
            return Ok(loginResult);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            var registrationResult = await _authService.RegisterUser(userRegistrationDto);
            if (!registrationResult.Success)
            {
                return BadRequest(new { Errors = registrationResult.Errors });
            }
            return Ok(registrationResult);
        }

        [HttpGet("c")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var result = await _authService.ConfirmEmailAsync(token, email);
            if (result)
            {
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Incorrect token or identification.");
            }
        }
    }
}
