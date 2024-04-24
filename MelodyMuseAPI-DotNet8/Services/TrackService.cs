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
        private readonly OpenAIApiService _openAIApiService;
        private readonly ModelService _modelService;
        public TrackService(MongoDbService mongoDbService, OpenAIApiService openAIApiService, ModelService modelService)
        {
            _mongoDbService = mongoDbService;
            _openAIApiService = openAIApiService;
            _modelService = modelService;  
        }

        public async Task<string> GenerateTrack(TrackCreationDto trackCreationDto, string userId)
        {
            var metadata = await _openAIApiService.GetMetadataFromPrompt(trackCreationDto.Prompt, userId);

            var trackGenerationDto = new TrackGenerationDto
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
               
                var audioStram = await _modelService.GenerateAudioAsync(trackGenerationDto);
                var imageStram = await _openAIApiService.GenerateImageFileFromPrompt(trackCreationDto.Prompt, userId);
                var trackId = await _mongoDbService.AddTrackAsync(trackGenerationDto, imageStram, audioStram);

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
            return await _mongoDbService.DeleteTrackAsync(trackId);
        }

        public async Task<IEnumerable<Track>> SearchTracks(string searchTerm)
        {
            return await _mongoDbService.SearchTracksAsync(searchTerm);
        }

        public async Task<Stream> GetMediaById(string mediaId, string type)
        {
            if (!ObjectId.TryParse(mediaId, out ObjectId fileId))
            {
                throw new ArgumentException("Invalid media ID");
            }

            return await _mongoDbService.GetFileByIdAsync(fileId);
        }
    }

}
