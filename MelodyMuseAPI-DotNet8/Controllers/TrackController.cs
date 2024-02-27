using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MelodyMuseAPI_DotNet8.Controllers
{
    [ApiController]
    [Route("api/tracks")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TrackController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        // POST: api/tracks/generate
        [HttpPost("generate")]
        public async Task<ActionResult<string>> GenerateTrack([FromBody] TrackCreationDto trackCreationDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unauthorized Access.");
            }

            var trackId = await _trackService.GenerateTrack(trackCreationDto, userId);
            return Ok(new { trackId = trackId });
        }

        // GET: api/tracks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Track>> GetTrackById(string id)
        {
            var track = await _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound($"Track with ID {id} not found.");
            }
            return Ok(track);
        }

        // GET: api/tracks/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Track>>> GetTracksByUser(string userId)
        {
            var tracks = await _trackService.GetTracksByUser(userId);
            return Ok(tracks);
        }

        // DELETE: api/tracks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(string id)
        {
            var result = await _trackService.DeleteTrack(id);
            if (!result)
            {
                return NotFound($"Track with ID {id} not found.");
            }
            return NoContent();
        }

        // POST: api/tracks/search
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<Track>>> SearchTracks([FromBody] string searchTerm)
        {
            var tracks = await _trackService.SearchTracks(searchTerm);
            return Ok(tracks);
        }
    }
}
