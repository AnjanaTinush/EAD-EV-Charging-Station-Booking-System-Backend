using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    [BsonIgnoreExtraElements]  // 👈 This ignores _id automatically
    public class User
    {
        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }
    }
}
