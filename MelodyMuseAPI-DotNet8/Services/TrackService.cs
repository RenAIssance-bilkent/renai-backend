using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace MelodyMuseAPI_DotNet8.Services
{
    public class TrackService : ITrackService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly OpenAIApiService _openAIApiService;
        private readonly ModelApiService _modelApiService;
        public TrackService(MongoDbService mongoDbService, OpenAIApiService openAIApiService, ModelApiService modelApiService)
        {
            _mongoDbService = mongoDbService;
            _openAIApiService = openAIApiService;
            _modelApiService = modelApiService;
        }

        public async Task<string> GenerateTrack(TrackCreationDto trackCreationDto, string userId)
        {
            var metadata = _openAIApiService.GetMetadataFromPrompt(trackCreationDto.Prompt);

            var newTrackGen = new TrackModelGenerationDto
            {
                UserId = userId,
                Title = trackCreationDto.Title,
                Genre = trackCreationDto.Genre,
                CreatedAt = DateTime.UtcNow,
                Metadata = metadata,
            };

            var trackId = await _modelApiService.GenerateTrackAsync(newTrackGen);
            return trackId;
        }

        public async Task<Track> GetTrackById(string trackId)
        {
            return await _mongoDbService.GetTrackByIdAsync(trackId);
        }

        public async Task<IEnumerable<Track>> GetTracksByUser(string userId)
        {
            return await _mongoDbService.GetTracksByUserAsync(userId);
        }

        public async Task<bool> DeleteTrack(string trackId)
        {
            return await _mongoDbService.DeleteTrackAsync(trackId);
        }

        public async Task<IEnumerable<Track>> SearchTracks(string searchTerm)
        {
            return await _mongoDbService.SearchTracksAsync(searchTerm);
        }
    }

}
