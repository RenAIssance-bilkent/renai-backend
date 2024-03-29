﻿using MelodyMuseAPI.Models;
using MelodyMuseAPI.Dtos;

namespace MelodyMuseAPI.Interfaces
{
    public interface ITrackService
    {
        Task<string> GenerateTrack(TrackCreationDto trackCreationDto, string userId);
        Task<Track> GetTrackById(string trackId);
        Task<IEnumerable<Track>> GetTracksByUser(string userId);
        Task<bool> DeleteTrack(string trackId);
        Task<IEnumerable<Track>> SearchTracks(string searchTerm);

    }
}
