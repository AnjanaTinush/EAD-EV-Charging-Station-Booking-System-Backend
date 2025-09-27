using Ev_backend.Models;

namespace Ev_backend.DTOs
{
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
    }

    // 👇 New DTO for Update (no Password field)
    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
    }
}
