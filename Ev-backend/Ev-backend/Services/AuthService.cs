using Ev_backend.Models;
using Ev_backend.Repositories;
using BCrypt.Net; // for password hashing

namespace Ev_backend.Services
{
    public class AuthService
    {
        private readonly AuthRepository _authRepository;

        public AuthService(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<User?> LoginAsync(string nic, string password)
        {
            var user = await _authRepository.ValidateUserAsync(nic, password);

            // Optional: If ValidateUserAsync does plain text check,
            // you can instead fetch user and verify hash here
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                return user;

            return null;
        }

        public async Task<User> RegisterAsync(User user)
        {
            // Ensure email uniqueness
            var existingUserByEmail = await _authRepository.GetByEmailAsync(user.Email);
            if (existingUserByEmail != null)
                throw new Exception("Email already exists!");

            // Ensure NIC uniqueness
            var existingUserByNIC = await _authRepository.GetByOwnerNICAsync(user.NIC);
            if (existingUserByNIC != null)
                throw new Exception("NIC already exists!");

            // Assign default role if none provided
            if (!Enum.IsDefined(typeof(UserRole), user.Role) || user.Role == 0)
            {
                user.Role = UserRole.StationOperator;
            }

            // ✅ Encrypt password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _authRepository.CreateAsync(user);
            return user;
        }
    }
}
