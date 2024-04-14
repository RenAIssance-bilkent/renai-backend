using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

namespace MelodyMuseAPI.Controllers
{
    [ApiController]
    [Route("api/t")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TrackController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly AudioService _audioService;

        public TrackController(ITrackService trackService, AudioService audioService)
        {
            _trackService = trackService;
            _audioService = audioService;
        }

        // POST: api/t/generate
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

        // GET: api/t/audio/{id}
        [HttpGet("audio/{id}")]
        public async Task<IActionResult> GetAudioById(string id)
        {
            var track = await _trackService.GetTrackById(id);
            if (track == null || string.IsNullOrEmpty(track.AudioURL))
            {
                return NotFound("Audio file not found.");
            }

            var audioStream = await _audioService.DownloadAudioAsync(new ObjectId(track.AudioURL));
            if (audioStream == null)
            {
                return NotFound("Audio file not found.");
            }

            return File(audioStream, "audio/wav");
        }

        // GET: api/t/{id}
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

        // GET: api/t/u/{userId}
        [HttpGet("u/{userId}")]
        public async Task<ActionResult<IEnumerable<Track>>> GetTracksByUser(string userId)
        {
            var tracks = await _trackService.GetTracksByUser(userId);
            return Ok(tracks);
        }

        // DELETE: api/t/{id}
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

        // POST: api/t/search
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<Track>>> SearchTracks([FromBody] string searchTerm)
        {
            var tracks = await _trackService.SearchTracks(searchTerm);
            return Ok(tracks);
        }
    }
}
