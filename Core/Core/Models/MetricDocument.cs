using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models
{
    public class MetricDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public Payload Payload { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}