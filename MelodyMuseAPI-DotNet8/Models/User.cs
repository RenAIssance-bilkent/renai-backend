#nullable disable

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MelodyMuseAPI.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        [JsonIgnore]
        public string PasswordHash { get; set; }

        [BsonElement("isConfermed")]
        public bool IsConfirmed { get; set; }

        [BsonElement("confirmationToken")]
        [JsonIgnore]
        public string ConfirmationToken { get; set; }

        // TODO: Add those fields to actual database

        [BsonElement("tracks")]
        public List<string> TrackIds { get; set; } = new List<string>();

        [BsonElement("points")]
        public int Points { get; set; }
    }
}
