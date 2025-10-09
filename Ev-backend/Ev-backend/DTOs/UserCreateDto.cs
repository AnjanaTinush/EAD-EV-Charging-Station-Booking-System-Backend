using Ev_backend.Models;

namespace Ev_backend.DTOs
{
    public class UserCreateDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public UserRole? Role { get; set; }   // ✅ optional → defaults to Backoffice
    }
}
