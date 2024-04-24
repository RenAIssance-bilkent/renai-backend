using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Cors;

namespace MelodyMuseAPI.Controllers
{
    [ApiController]
    [Route("api/u")]
    [EnableCors("AllowWebApp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly EmailSenderService _emailSenderService;

        public UserController(IUserService userService, EmailSenderService emailSenderService)
        {
            _userService = userService;
            _emailSenderService = emailSenderService;
        }

        // GET: api/u
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        // GET: api/u/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || currentUserId != id)
            {
                return Unauthorized("Unauthorized Access.");
            }

            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        // PUT: api/u/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserProfileUpdateDto updateDto)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || currentUserId != id)
            {
                return Unauthorized("Unauthorized Access.");
            }

            var result = await _userService.UpdateUser(id, updateDto);
            if (!result)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return NoContent();
        }

        // DELETE: api/u/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || currentUserId != id)
            {
                return Unauthorized("Unauthorized Access.");
            }

            var result = await _userService.DeleteUser(id);
            if (!result)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return NoContent();
        }

        // POST: api/u/purchase
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePoints(PurchasePointsDto ppdto)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || currentUserId != ppdto.UserId)
            {
                return Unauthorized("Unauthorized Access.");
            }

            try
            {
                var result = await _userService.PurchasePoints(ppdto.UserId, ppdto.Points);
                return Ok(result);

            } catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}