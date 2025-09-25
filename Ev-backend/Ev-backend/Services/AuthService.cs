using Ev_backend.Models;
using Ev_backend.Repositories;

namespace Ev_backend.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null || user.Password != password)
                return null;

            return user;
        }

        public async Task<User> RegisterAsync(User user)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new Exception("User already exists!");
            }

            await _userRepository.CreateAsync(user);
            return user;
        }
    }
}
