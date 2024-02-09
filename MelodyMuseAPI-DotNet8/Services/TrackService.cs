using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Dtos;
using MelodyMuseAPI_DotNet8.Interfaces;

namespace MelodyMuseAPI_DotNet8.Services
{
    public class TrackService : ITrackService
    {
        private readonly MongoDBService _mongoDBService;

        public TrackService(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // TODO: Implement model interaction
        public async Task<Track> GenerateTrack(TrackCreationDto trackCreationDto, string userId)
        {
            var newTrack = new Track
            {
                //TODO: Track title should be generated
                Title = trackCreationDto.Title,
                Genre = trackCreationDto.Genre,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                AudioURL = trackCreationDto.AudioURL,
                Metadata = trackCreationDto.Metadata
            };

            await _mongoDBService.AddTrackAsync(newTrack);
            return newTrack;
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
