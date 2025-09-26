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
            var existingUser = await _authRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already exists!");
            }

            await _authRepository.CreateAsync(user);
            return user;
        }
    }
}
