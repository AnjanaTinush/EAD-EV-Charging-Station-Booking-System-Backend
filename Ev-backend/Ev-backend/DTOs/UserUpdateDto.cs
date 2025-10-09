using Ev_backend.Models;

namespace Ev_backend.DTOs
{
    public class UserUpdateDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public UserRole? Role { get; set; }   // ✅ make nullable (fixes HasValue / ?? issues)
    }
}
