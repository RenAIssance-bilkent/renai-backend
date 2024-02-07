using MelodyMuseAPI.Models;
using MelodyMuseAPI_DotNet8.Data;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace MelodyMuseAPI_DotNet8.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _userCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var securePassphrase = new SecureString();
            foreach (char c in "dev")
            {
                securePassphrase.AppendChar(c);
            }

            // Make sure to mark the SecureString as read-only after modifying it
            securePassphrase.MakeReadOnly();

            var connectionString = mongoDBSettings.Value.ConnectionURI;
            var settings = MongoClientSettings.FromConnectionString(connectionString);

            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var cert = new X509Certificate2(mongoDBSettings.Value.CertificatePath, securePassphrase);

            settings.SslSettings = new SslSettings
            {
                ClientCertificates = new List<X509Certificate>() { cert }
            };
            var client = new MongoClient(settings);

            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);

            _userCollection = database.GetCollection<User>("users"); // collection name in data sample
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userCollection.Find(new BsonDocument()).ToListAsync();
        }
    }
}
