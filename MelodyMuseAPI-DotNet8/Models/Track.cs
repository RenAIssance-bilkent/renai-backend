﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MelodyMuseAPI.Models
{
    public class Track
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("genre")]
        public string Genre { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("audioId")]
        public ObjectId AudioId { get; set; }

        [BsonElement("imageId")]
        public ObjectId ImageId { get; set; }

        [BsonElement("metadata")]
        public Metadata Metadata { get; set; }

        [BsonElement("model")]
        public int Model { get; set; }
    }
}
