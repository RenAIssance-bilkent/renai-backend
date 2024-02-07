#nullable disable

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

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
        public string PasswordHash { get; set; }

        // TODO: Add those fields to actual database

        //[BsonElement("Tracks")]
        //public List<string> TrackIds { get; set; } = new List<string>();

        //[BsonElement("Points")]
        //public int Points { get; set; }
    }
}
