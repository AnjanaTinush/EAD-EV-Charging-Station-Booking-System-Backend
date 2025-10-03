namespace Ev_backend.DTOs
{
    public class CreateEVOwnerDto
    {
        public string NIC { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
    }
}
