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
        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        [BsonElement("role")]
        public UserRole Role { get; set; }
    }
}
