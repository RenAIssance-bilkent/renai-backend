using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using MongoDB.Bson;

namespace MelodyMuseAPI.Services
{
    public class TrackService : ITrackService
    {
        private readonly MongoDbService _mongoDbService;
        private readonly AudioService _audioService;
        private readonly OpenAIApiService _openAIApiService;
        private readonly ModelService _modelService;
        public TrackService(MongoDbService mongoDbService, OpenAIApiService openAIApiService, ModelService modelService, AudioService audioService)
        {
            _mongoDbService = mongoDbService;
            _openAIApiService = openAIApiService;
            _modelService = modelService;
            _audioService = audioService;   
        }

        public async Task<string> GenerateTrack(TrackCreationDto trackCreationDto, string userId)
        {
            var metadata = await _openAIApiService.GetMetadataFromPrompt(trackCreationDto.Prompt, userId);

            var newTrackGen = new TrackModelGenerationDto
            {
                UserId = userId,
                Title = trackCreationDto.Title,
                Genre = trackCreationDto.Genre,
                CreatedAt = DateTime.UtcNow,
                Metadata = metadata,
            };

            var isEnoughPoints = await _mongoDbService.ReduceUserPoints(userId, metadata.Length); //TODO: Calculate points, can be hardcoded.

            if (isEnoughPoints)
            {
                var trackId = await _modelService.GenerateTrackAsync(newTrackGen);
                return trackId;
            }

            throw new Exception("Not enough points."); 
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
            var track = await _mongoDbService.GetTrackByIdAsync(trackId);
            if (track != null && !string.IsNullOrEmpty(track.AudioURL))
            {
                // AudioURL is the string representation of ObjectId
                await _audioService.DeleteAudioAsync(new ObjectId(track.AudioURL));
            }
            return await _mongoDbService.DeleteTrackAsync(trackId);
        }

        public async Task<IEnumerable<Track>> SearchTracks(string searchTerm)
        {
            return await _mongoDbService.SearchTracksAsync(searchTerm);
        }
    }

}
