using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    public class EVOwner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nic")]
        public string NIC { get; set; }   // Primary Key

        [BsonElement("fullName")]
        public string FullName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
