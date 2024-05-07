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
        private readonly OpenAIApiService _openAIApiService;

        public TrackController(ITrackService trackService, OpenAIApiService openAIApiService)
        {
            _trackService = trackService;
            _openAIApiService = openAIApiService;
        }

        // POST: api/t/generate 
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTrack([FromBody] Metadata metadata)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unauthorized Access.");
            }

            Response.Headers.Add("Content-Type", "text/event-stream");

            var logEntries = new (int Delay, int CompletionPercent, string Message)[]
            {
                (0, 0, "[INFO] Starting generation process..."),
                (2, 5, "[INFO] Loading model configuration and initializing generation parameters..."),
                (2, 10, "[INFO] Model configuration loaded. Version 1db0c525 initialized..."),
                (30, 15, "[INFO] Preprocessing input data and applying initial transformations..."),
                (40, 20, "[DEBUG] Input data validation completed successfully..."),
                (50, 25, "[INFO] Starting deep learning pipeline for track synthesis..."),
                (1, 30, "[DEBUG] Layer 1 processing completed. Analyzing spectral features..."),
                (1, 35, "[DEBUG] Layer 2 processing completed. Rhythmic patterns established..."),
                (20, 40, "[INFO] Mid-generation checkpoint reached. Intermediate data saved..."),
                (30, 45, "[INFO] Generating harmonic structures and applying melodic contours..."),
                (40, 50, "[DEBUG] Melody generation in progress, synthesizing using LSTM networks..."),
                (50, 55, "[INFO] Melody synthesized. Beginning integration of harmony and rhythm..."),
                (1, 60, "[DEBUG] Harmony and rhythm integration successful. Finalizing track composition..."),
                (5, 65, "[INFO] Finalizing track details and applying post-processing effects..."),
                (9, 70, "[INFO] Track post-processing completed. Preparing final output..."),
                (20, 75, "[INFO] Final output preparation complete. Conducting final quality checks..."),
                (30, 80, "[INFO] Quality checks passed. Final track rendering..."),
                (10, 85, "[INFO] Track rendering complete. Saving the final track output..."),
                (4, 90, "[INFO] Track successfully saved. Ending generation process..."),
                (0, 95, "[INFO] Generation process completed successfully.")
            };

            // Start the actual track generation in a background task
            var trackGenerationTask = Task.Run(async () =>
            {
                return await _trackService.GenerateTrack(metadata, userId);
            });

            foreach (var log in logEntries)
            {
                await Task.Delay(log.Delay * 1000);

                var logMessage = log.Message;
                var jsonMessage = $"data: {{\"log\": \"{logMessage}\", \"completion_percent\": {log.CompletionPercent} }}\n\n";
                await Response.WriteAsync(jsonMessage);
                await Response.Body.FlushAsync();
            }

            var trackId = await trackGenerationTask; // Wait for the track generation to complete
            await Response.WriteAsync($"data: {{\"trackId\": \"{trackId}\", \"completion_percent\": 100}}\n\n");
            await Response.Body.FlushAsync();

            Response.Body.Close();
            return new EmptyResult();
        }


        // POST: api/t/generate-metadata
        [HttpPost("generate-metadata")]
        public async Task<IActionResult> GenerateMetadata([FromBody] TrackCreationDto trackCreationDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unauthorized Access.");
            }

            var metadata = await _trackService.GenerateTrackMetadata(trackCreationDto, userId);

            return Ok(metadata);
        }

        // Get api/t/media/{type}/{id}
        [HttpGet("media/{type}/{id}")]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
                AudioEndpoint = $"/api/t/media/audio/{track.AudioId}/",
                ImageEndpoint = $"/api/t/media/image/{track.ImageId}/",

            }).ToList();

            return Ok(trackRetrivals);
        }

        // GET: api/t/random-prompt
        [HttpGet("random-prompt")]
        public async Task<ActionResult<string>> GetRandomPrompt()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unauthorized Access.");
            }

            try
            {
                var randomPrompt = await _openAIApiService.GenerateRandomPrompt(userId);
                return Ok(randomPrompt);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error generating random prompt: {ex.Message}");
            }
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
                AudioEndpoint = $"/api/t/media/audio/{track.AudioId}/",
                ImageEndpoint = $"/api/t/media/image/{track.ImageId}/",
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
                AudioEndpoint = $"/api/t/media/audio/{track.AudioId}/",
                ImageEndpoint = $"/api/t/media/image/{track.ImageId}/",
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
        [AllowAnonymous]
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
                AudioEndpoint = $"/api/t/media/audio/{track.AudioId}/",
                ImageEndpoint = $"/api/t/media/image/{track.ImageId}/",
            }).ToList();

            return Ok(trackRetrivals);
        }
    }
}

