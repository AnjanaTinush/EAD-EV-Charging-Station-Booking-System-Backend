using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    public class Station
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }   // AC / DC

        [BsonElement("availableSlots")]
        public int AvailableSlots { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
