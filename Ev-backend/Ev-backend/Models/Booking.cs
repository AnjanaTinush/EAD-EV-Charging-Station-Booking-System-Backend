using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ev_backend.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ownerNic")]
        public string OwnerNIC { get; set; }   // Links to EVOwner.NIC

        [BsonElement("stationId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; }  // Links to Station.Id

        [BsonElement("reservationTime")]
        public DateTime ReservationTime { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Pending";
        // Pending, Approved, Cancelled
    }
}
