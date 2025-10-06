using Ev_backend.Models;
using Ev_backend.Repositories;
using Ev_backend.Models.Enums;

namespace Ev_backend.Services
{
    public class AuthService
    {
        private readonly AuthRepository _authRepository;

        public AuthService(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // =================== LOGIN ===================
        public async Task<User?> LoginAsync(string nic, string password)
        {
            // ✅ Step 1: Fetch user by NIC
            var user = await _authRepository.GetByNICAsync(nic);

            // ✅ Step 2: Check if user exists and verify password hash
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            // ❌ Invalid NIC or password
            return null;
        }

        // =================== REGISTER ===================
        public async Task<User> RegisterAsync(User user)
        {
            // ✅ Ensure Email is unique
            var existingEmail = await _authRepository.GetByEmailAsync(user.Email);
            if (existingEmail != null)
                throw new Exception("Email already exists!");

            // ✅ Ensure NIC is unique
            var existingNIC = await _authRepository.GetByNICAsync(user.NIC);
            if (existingNIC != null)
                throw new Exception("NIC already exists!");

            // ✅ Assign default role if not set
            if (!Enum.IsDefined(typeof(UserRole), user.Role))
                user.Role = UserRole.StationOperator;

            // ✅ Hash the password before storing
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _authRepository.CreateAsync(user);
            return user;
        }
    }
}
