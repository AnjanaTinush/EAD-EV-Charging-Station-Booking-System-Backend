using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    public enum UserRole
    {
        Backoffice,
        StationOperator,
        EvOwner
    }

    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }   // 👈 Nullable, MongoDB will auto-generate

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("ownerNIC")]
        public string OwnerNIC { get; set; }   // 👈 NEW required & unique

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        [BsonElement("role")]
        public UserRole Role { get; set; }
    }
}
