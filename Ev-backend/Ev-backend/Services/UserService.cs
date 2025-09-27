using Ev_backend.Models;
using Ev_backend.Repositories;

namespace Ev_backend.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<List<User>> GetAllAsync() => _userRepository.GetAllAsync();

        public Task<User?> GetByIdAsync(string id) => _userRepository.GetByIdAsync(id);

        public Task CreateAsync(User user) => _userRepository.CreateAsync(user);

        public Task UpdateAsync(string id, User user) => _userRepository.UpdateAsync(id, user);

        public Task DeleteAsync(string id) => _userRepository.DeleteAsync(id);
    }
}
