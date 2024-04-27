using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using MelodyMuseAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;

namespace MelodyMuseAPI.Controllers
{
    [ApiController]
    [Route("api/t")]
    [EnableCors("AllowWebApp")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TrackController : Controller
    {
        private readonly ITrackService _trackService;

        public TrackController(ITrackService trackService)
        {
            _trackService = trackService;
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

        // Get api/t/media/{type}/{id}
        [HttpGet("media/{type}/{id}")]
        public async Task<IActionResult> GetMediaById(string type, string id)
        {
            var stream = await _trackService.GetMediaById(id, type);
            if (stream == null)
            {
                return NotFound($"{type} not found.");
            }

            var contentType = type == "audio" ? "audio/wav" : "image/jpeg"; // Assuming default types
            return File(stream, contentType);
        }

        // GET: api/t
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<TrackRetrivalDto>>> GetAllTracks()
        {
            var tracks = await _trackService.GetAllTracks();

            var trackRetrivals = tracks.Select(track => new TrackRetrivalDto()
            {
                Id = track.Id,
                UserId = track.UserId,
                Title = track.Title,
                Genre = track.Genre,
                CreatedAt = track.CreatedAt,
                Metadata = track.Metadata,
                Model = track.Model,
                AudioEndpoint = $"api/t/media/audio/{track.AudioId}/",
                ImageEndpoint = $"api/t/media/image/{track.ImageId}/",

            }).ToList();

            return Ok(trackRetrivals);
        }

        // GET: api/t/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TrackRetrivalDto>> GetTrackById(string id)
        {
            var track = await _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound($"Track with ID {id} not found.");
            }

            var trackRetrival = new TrackRetrivalDto()
            {
                Id = track.Id,
                UserId = track.UserId,
                Title = track.Title,
                Genre = track.Genre,
                CreatedAt = track.CreatedAt,
                Metadata = track.Metadata,
                Model = track.Model,
                AudioEndpoint = $"api/t/{track.AudioId}/audio",
                ImageEndpoint = $"api/t/{track.ImageId}/image",
            };
            return Ok(trackRetrival);
        }

        // GET: api/t/u/{userId}
        [HttpGet("u/{userId}")]
        public async Task<ActionResult<IEnumerable<TrackRetrivalDto>>> GetTracksByUser(string userId)
        {
            var tracks = await _trackService.GetTracksByUser(userId);
            if (!tracks.Any())
            {
                return NotFound($"No tracks found for user ID {userId}.");
            }

            var trackRetrivals = tracks.Select(track => new TrackRetrivalDto()
            {
                Id = track.Id,
                UserId = track.UserId,
                Title = track.Title,
                Genre = track.Genre,
                CreatedAt = track.CreatedAt,
                Metadata = track.Metadata,
                Model = track.Model,
                AudioEndpoint = $"api/t/{track.AudioId}/audio",
                ImageEndpoint = $"api/t/{track.ImageId}/image",
            }).ToList();

            return Ok(trackRetrivals);
        }

        // DELETE: api/t/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(string id)
        {
            bool result = await _trackService.DeleteTrack(id);
            if (!result)
            {
                return NotFound($"Track with ID {id} not found.");
            }
            return NoContent();
        }

        // POST: api/t/search
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<TrackRetrivalDto>>> SearchTracks([FromBody] string title)
        {
            var tracks = await _trackService.SearchTracks(title);
            if (!tracks.Any())
            {
                return NotFound($"No tracks found matching '{title}'.");
            }

            var trackRetrivals = tracks.Select(track => new TrackRetrivalDto()
            {
                Id = track.Id,
                UserId = track.UserId,
                Title = track.Title,
                Genre = track.Genre,
                CreatedAt = track.CreatedAt,
                Metadata = track.Metadata,
                Model = track.Model,
                AudioEndpoint = $"api/t/{track.AudioId}/audio",
                ImageEndpoint = $"api/t/{track.ImageId}/image",
            }).ToList();

            return Ok(trackRetrivals);
        }
    }
}

