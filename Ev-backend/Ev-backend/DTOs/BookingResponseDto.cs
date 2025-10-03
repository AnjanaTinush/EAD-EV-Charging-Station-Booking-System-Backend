using Ev_backend.Models.Enums;

namespace Ev_backend.DTOs
{
    public class BookingResponseDto
    {
        public string Id { get; set; } = default!;
        public string OwnerNIC { get; set; } = default!;
        public string StationId { get; set; } = default!;
        public DateTime ReservationTime { get; set; }
        public BookingStatus Status { get; set; }
        public string? QrCodeBase64 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
