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

        public async Task<User?> LoginAsync(string email, string password)
        {
            return await _authRepository.ValidateUserAsync(email, password);
        }

        public async Task<User> RegisterAsync(User user)
        {
            var existingUserByEmail = await _authRepository.GetByEmailAsync(user.Email);
            if (existingUserByEmail != null)
                throw new Exception("Email already exists!");

            var existingUserByNIC = await _authRepository.GetByOwnerNICAsync(user.NIC);
            if (existingUserByNIC != null)
                throw new Exception("OwnerNIC already exists!");

            await _authRepository.CreateAsync(user);
            return user;
        }

    }
}
