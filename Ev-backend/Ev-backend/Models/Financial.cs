using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    public class Financial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("nic")]
        public string NIC { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("paymentType")]
        public string PaymentType { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending"; // default status
    }
}
