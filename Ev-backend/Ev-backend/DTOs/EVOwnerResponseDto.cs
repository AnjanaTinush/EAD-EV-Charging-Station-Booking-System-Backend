namespace Ev_backend.DTOs
{
    public class EVOwnerResponseDto
    {
        public string Id { get; set; } = default!;
        public string NIC { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
