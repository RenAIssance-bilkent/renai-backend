using MelodyMuseAPI.Dtos;
using MelodyMuseAPI.Models;
using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace MelodyMuseAPI.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Track> _trackCollection;
        private readonly IGridFSBucket _gridFSBucket;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var connectionString = mongoDbSettings.Value.ConnectionURI;
            var settings = MongoClientSettings.FromConnectionString(connectionString);

            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);

            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);

            _userCollection = database.GetCollection<User>("users"); // Adjust the collection name as necessary
            _trackCollection = database.GetCollection<Track>("tracks"); // Adjust the collection name as necessary
            _gridFSBucket = new GridFSBucket(database);
        }


        #region User Handling
        // This function is for testing, remove on deploy
        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userCollection.Find(new BsonDocument()).ToListAsync();
            return users.ConvertAll(user => ToDto(user));
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
            return ToDto(user);
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userCollection.Find(user => user.Id == userId).FirstOrDefaultAsync();
            return ToDto(user);
        }

        public async Task AddUserAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
        }
        public async Task<bool> UpdateUserAsync(string userId, UpdateDefinition<User> updateDefinition)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var result = await _userCollection.UpdateOneAsync(filter, updateDefinition);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        public async Task<bool> UpdateUserPasswordAsync(string userId, string newPassword)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            var update = Builders<User>.Update.Set(user => user.PasswordHash, BCrypt.Net.BCrypt.HashPassword(newPassword));
            var result = await _userCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
        public async Task<DeleteResult> DeleteUserAsync(string userId)
        {
            return await _userCollection.DeleteOneAsync(user => user.Id == userId);
        }
        public async Task<bool> ReduceUserPoints(string userId, int points)
        {
            if (points < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(points), "Points to reduce cannot be negative.");
            }

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Inc(u => u.Points, -points);
            var result = await _userCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
        public async Task<bool> AddUserPoints(string userId, int points)
        {

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Inc(u => u.Points, points);
            var result = await _userCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
        public async Task<bool> ValidateConfirmationTokenAsync(string userEmail, string token)
        {
            var user = await _userCollection.Find(user => user.Email == userEmail).FirstOrDefaultAsync();

            if (user == null || user.ConfirmationToken != token || user.IsConfirmed)
                return false;

            var updateDefinition = Builders<User>.Update
                .Set(u => u.IsConfirmed, true)
                .Set(u => u.ConfirmationToken, ""); // or Unset

            var updateResult = await _userCollection.UpdateOneAsync(
                Builders<User>.Filter.Eq(u => u.Id, user.Id),
                updateDefinition);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> ValidatePasswordHashByIdAsync(string userId, string password)
        {
            var user = await _userCollection.Find(user => user.Id == userId)
                                            .FirstOrDefaultAsync();

            if (user == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<bool> ValidatePasswordHashByEmailAsync(string userEmail, string password)
        {
            var user = await _userCollection.Find(user => user.Email == userEmail)
                                            .FirstOrDefaultAsync();

            if (user == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        private UserDto ToDto(User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Points = user.Points,
                TrackIds = user.TrackIds
            };
        }
        #endregion

        #region Task Handling

        public async Task<string> AddTrackAsync(TrackGenerationDto trackDto, Stream imageStream, Stream audioStream)
        {
            var track = new Track()
            {
                Title = trackDto.Title,
                UserId = trackDto.UserId,
                CreatedAt = trackDto.CreatedAt,
                Genre = trackDto.Genre,
                Metadata = trackDto.Metadata,
                Model = trackDto.Model,
                ImageId = await UploadTrackImageAsync(imageStream, trackDto.Title), // Instead of name is title of the track
                AudioId = await UploadAudioFileAsync(audioStream, trackDto.Title)
            };

            await _trackCollection.InsertOneAsync(track);

            return track.Id;
        }

        public async Task<IEnumerable<Track>> GetAllTracksAsync()
        {
            return await _trackCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Track> GetTrackByIdAsync(string trackId)
        {
            return await _trackCollection.Find(t => t.Id == trackId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Track>> GetTracksByUserAsync(string userId)
        {
            return await _trackCollection.Find(t => t.UserId == userId).ToListAsync();
        }

        public async Task<bool> DeleteTrackAsync(string trackId)
        {
            var track = await GetTrackByIdAsync(trackId);
            if (track != null)
            {
                if (track.ImageId != ObjectId.Empty)
                    await DeleteTrackImageAsync(track.ImageId);
                if (track.AudioId != ObjectId.Empty)
                    await DeleteAudioFileAsync(track.AudioId);

                var result = await _trackCollection.DeleteOneAsync(t => t.Id == trackId);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Track>> SearchTracksAsync(string searchTerm)
        {
            var filter = Builders<Track>.Filter.Regex("title", new BsonRegularExpression(searchTerm, "i"));
            return await _trackCollection.Find(filter).ToListAsync();
        }

        #endregion


        #region Media Handling

        public async Task<ObjectId> UploadTrackImageAsync(Stream imageStream, string imageName)
        {
            if (imageStream.CanSeek)
                imageStream.Seek(0, SeekOrigin.Begin);

            var imageId = await _gridFSBucket.UploadFromStreamAsync(imageName, imageStream);
            return imageId;
        }

        public async Task<ObjectId> UploadAudioFileAsync(Stream audioStream, string audioName)
        {
            if (audioStream.CanSeek)
                audioStream.Seek(0, SeekOrigin.Begin);
            var audioId = await _gridFSBucket.UploadFromStreamAsync(audioName, audioStream);
            return audioId;
        }

        public async Task DeleteTrackImageAsync(ObjectId imageId)
        {
            try
            {
                await _gridFSBucket.DeleteAsync(imageId);
            }
            catch (GridFSFileNotFoundException ex)
            {
                Console.WriteLine($"Failed to delete image: {ex.Message}");
            }
        }

        public async Task DeleteAudioFileAsync(ObjectId audioId)
        {
            try
            {
                await _gridFSBucket.DeleteAsync(audioId);
            }
            catch (GridFSFileNotFoundException ex)
            {
                Console.WriteLine($"Failed to delete audio: {ex.Message}");
            }
        }

        public async Task<Stream> GetFileByIdAsync(ObjectId fileId)
        {
            try
            {
                return await _gridFSBucket.OpenDownloadStreamAsync(fileId);
            }
            catch (GridFSFileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return null;
            }
        }


        #endregion
    }
}
