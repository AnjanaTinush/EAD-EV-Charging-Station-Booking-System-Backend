using Ev_backend.Models;

namespace Ev_backend.DTOs
{
    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NIC { get; set; }
        public UserRole Role { get; set; }
    }
}
