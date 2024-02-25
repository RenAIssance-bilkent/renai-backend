using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;

namespace MelodyMuseAPI_DotNet8.Services
{
    public class TrackService : ITrackService
    {
        private readonly MongoDBService _mongoDBService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _modelAPIBaseURL; // TODO: Add URL 

        public TrackService(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // TODO: Implement model interaction
        //public async Task<Track> GenerateTrack(TrackCreationDto trackCreationDto, string userId)
        //{
        //    var newTrack = new Track
        //    {
        //        //TODO: Track title should be generated
        //        Title = trackCreationDto.Title,
        //        Genre = trackCreationDto.Genre,
        //        UserId = userId,
        //        CreatedAt = DateTime.UtcNow,
        //        AudioURL = trackCreationDto.AudioURL,
        //        Metadata = trackCreationDto.Metadata
        //    };

        //    await _mongoDBService.AddTrackAsync(newTrack);
        //    return newTrack;
        //}

        public async Task<Track> GenerateTrack(TrackCreationDto trackCreationDto, string userId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync($"{_modelAPIBaseURL}/generate-track", trackCreationDto);

            if (response.IsSuccessStatusCode)
            {
                var trackData = await response.Content.ReadFromJsonAsync<TrackCreationDto>();

                var track = new Track
                {
                    Title = trackData.Title,
                    Genre = trackData.Genre,
                    AudioURL = trackData.AudioURL,
                    Metadata = trackData.Metadata,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _mongoDBService.AddTrackAsync(track);
                return track;
            }
            else
            {
                throw new InvalidOperationException("Failed to generate track.");
            }
        }

        public async Task<Track> GetTrackById(string trackId)
        {
            return await _mongoDBService.GetTrackByIdAsync(trackId);
        }

        public async Task<IEnumerable<Track>> GetTracksByUser(string userId)
        {
            return await _mongoDBService.GetTracksByUserAsync(userId);
        }

        public async Task<bool> DeleteTrack(string trackId)
        {
            return await _mongoDBService.DeleteTrackAsync(trackId);
        }

        public async Task<IEnumerable<Track>> SearchTracks(string searchTerm)
        {
            return await _mongoDBService.SearchTracksAsync(searchTerm);
        }
    }

}
