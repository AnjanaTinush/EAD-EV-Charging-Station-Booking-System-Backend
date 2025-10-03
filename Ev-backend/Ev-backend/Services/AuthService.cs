using Ev_backend.Models;
using Ev_backend.Repositories;

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
            return await _authRepository.ValidateUserAsync(nic, password);
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

            await _authRepository.CreateAsync(user);
            return user;
        }
    }
}
