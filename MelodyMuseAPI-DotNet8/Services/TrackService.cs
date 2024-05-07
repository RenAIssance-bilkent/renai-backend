using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using MongoDB.Bson;
using System.IO;

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

        public async Task<Metadata> GenerateTrackMetadata(TrackCreationDto trackCreationDto, string userId)
        {
            if(trackCreationDto.Prompt != null)
            {
                var metadata = await _openAIApiService.GetMetadataFromPromptForReplica(trackCreationDto, userId);

                return metadata;
            }
            else if(trackCreationDto.Genre != null)
            {
                trackCreationDto.Prompt = await _openAIApiService.GeneratePromptBasedOnGenre(trackCreationDto.Genre, userId);
            }
            else
            {
                throw new ArgumentNullException("Invalid Input: Prompt or Genre has to be given.");
            }

            if(trackCreationDto.Model == 0)
            {
                var metadata = await _openAIApiService.GetMetadataFromPromptForReplica(trackCreationDto, userId);

                return metadata;
            }

            throw new Exception("Invalid Model Option");
           
        }

        public async Task<string> GenerateTrack(Metadata metadata, string userId)
        {
            var isEnoughPoints = await _mongoDbService.ReduceUserPoints(userId, 10); //TODO: Calculate points, can be hardcoded.

            if (!isEnoughPoints)
            {
                throw new Exception("Not enough points.");
            }

            if (metadata.Model == 0)
            {

                var trackGenerationDto = new TrackGenerationDto
                {
                    UserId = userId,
                    Title = metadata.Title,
                    Genre = metadata.Genre,
                    CreatedAt = DateTime.UtcNow,
                    Metadata = metadata,
                    Model = 0
                };

                var audioStram = await _modelService.GenerateWithReplicateAsync(trackGenerationDto);
                var imageStram = await _openAIApiService.GenerateImageFileFromPrompt(metadata.EnhancedPrompt, userId);
                var trackId = await _mongoDbService.AddTrackAsync(trackGenerationDto, imageStram, audioStram);

                return trackId;
            }

            throw new Exception("Invalid Model Option");

        }

        public async Task<IEnumerable<Track>> GetAllTracks()
        {
            return await _mongoDbService.GetAllTracksAsync();
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
