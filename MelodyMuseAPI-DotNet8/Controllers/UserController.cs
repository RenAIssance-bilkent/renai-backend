﻿using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using MelodyMuseAPI_DotNet8.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MelodyMuseAPI_DotNet8.Controllers
{
    [ApiController]
    [Route("api/u")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        // POST: api/u/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] UserRegistrationDto registrationDto)
        {
            var registeredUser = await _userService.RegisterUser(registrationDto);
            if (registeredUser == null)
            {
                return BadRequest("User registration failed. User may already exist.");
            }
            return CreatedAtAction(nameof(GetUserById), new { id = registeredUser.Id }, registeredUser);
        }

        // PUT: api/u/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserProfileUpdateDto updateDto)
        {
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
            var result = await _userService.DeleteUser(id);
            if (!result)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return NoContent();
        }
    }
}