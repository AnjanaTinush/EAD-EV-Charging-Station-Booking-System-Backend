using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Ev_backend.Models.Enums;

namespace Ev_backend.Models
{
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        // Foreign Key: EV Owner NIC
        [BsonElement("ownerNic")]
        public string OwnerNIC { get; set; } = default!;

        [BsonElement("stationId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; } = default!;

        [BsonElement("reservationTime")]
        public DateTime ReservationTime { get; set; }

        [BsonElement("status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("approvedAt")]
        public DateTime? ApprovedAt { get; set; }

        [BsonElement("cancelledAt")]
        public DateTime? CancelledAt { get; set; }

        [BsonElement("cancelReason")]
        public string? CancelReason { get; set; }

        [BsonElement("qrCodeBase64")]
        public string? QrCodeBase64 { get; set; }
    }
}
