using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    public class EVOwner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  // MongoDB will generate this automatically

        [BsonElement("nic")]
        public string NIC { get; set; } = string.Empty;

        [BsonElement("fullName")]
        public string FullName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty; // hashed password

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
