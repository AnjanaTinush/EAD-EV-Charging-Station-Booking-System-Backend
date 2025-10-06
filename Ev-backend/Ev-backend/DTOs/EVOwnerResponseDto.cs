namespace Ev_backend.DTOs
{
    public class EVOwnerResponseDto
    {
        public string? Id { get; set; }
        public string NIC { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
