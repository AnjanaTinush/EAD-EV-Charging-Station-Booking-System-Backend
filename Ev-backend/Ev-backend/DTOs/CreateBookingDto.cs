namespace Ev_backend.DTOs
{
    public class CreateBookingDto
    {
        public string OwnerNIC { get; set; } = default!;
        public string StationId { get; set; } = default!;
        public DateTime ReservationTime { get; set; }
    }
}
