using MelodyMuseAPI.Models;
using MelodyMuseAPI.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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


        #region UserCollection
        // This function is for testing, remove on deploy
        public async Task<List<User>> GetAllAsync()
        {
            return await _userCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userCollection.Find(user => user.Id == userId).FirstOrDefaultAsync();
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
        #endregion

        #region TaskCollection
        public async Task AddTrackAsync(Track track)
        {
            await _trackCollection.InsertOneAsync(track);
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
            var result = await _trackCollection.DeleteOneAsync(t => t.Id == trackId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        public async Task<IEnumerable<Track>> SearchTracksAsync(string searchTerm)
        {
            var filter = Builders<Track>.Filter.Regex("title", new BsonRegularExpression(searchTerm, "i"));
            return await _trackCollection.Find(filter).ToListAsync();
        }
        #endregion

        #region AudioBucket
        public async Task<ObjectId> UploadFileToGridFSAsync(Stream fileStream, string fileName)
        {
            var objectId = await _gridFSBucket.UploadFromStreamAsync(fileName, fileStream);
            return objectId;
        }

        public async Task<Stream> DownloadFileFromGridFSAsync(ObjectId id)
        {
            var stream = await _gridFSBucket.OpenDownloadStreamAsync(id);
            return stream;
        }

        public async Task DeleteFileFromGridFSAsync(ObjectId id)
        {
            await _gridFSBucket.DeleteAsync(id);
        }
        #endregion
    }
}
