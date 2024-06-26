﻿using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;

namespace MelodyMuseAPI.Interfaces
{
    public interface ITrackService
    {
        Task<string> GenerateTrack(Metadata metadata, string userId);
        Task<Metadata> GenerateTrackMetadata(TrackCreationDto trackCreationDto, string userId);
        Task<IEnumerable<Track>> GetAllTracks();
        Task<Track> GetTrackById(string trackId);
        Task<IEnumerable<Track>> GetTracksByUser(string userId);
        Task<IEnumerable<Track>> GetTracksByGenre(string genre);
        Task<bool> DeleteTrack(string trackId);
        Task<IEnumerable<Track>> SearchTracks(string searchTerm);
        Task<Stream> GetMediaById(string mediaId, string type);
    }
}
