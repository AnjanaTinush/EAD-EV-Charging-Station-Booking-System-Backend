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
        public Task<User?> GetByNICAsync(string nic) => _userRepository.GetByNICAsync(nic);
        public Task<User?> GetByEmailAsync(string email) => _userRepository.GetByEmailAsync(email);

        // ✅ Account state management
        public Task DeactivateAsync(string id) => _userRepository.SetActiveStatusAsync(id, false);
        public Task ActivateAsync(string id) => _userRepository.SetActiveStatusAsync(id, true);

        // ✅ Existence checks
        public Task<bool> ExistsByNICAsync(string nic) => _userRepository.ExistsByNICAsync(nic);
        public Task<bool> ExistsByEmailAsync(string email) => _userRepository.ExistsByEmailAsync(email);
    }
}
