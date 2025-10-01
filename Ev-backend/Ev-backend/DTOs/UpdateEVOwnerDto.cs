namespace Ev_backend.DTOs
{
    public class UpdateEVOwnerDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
