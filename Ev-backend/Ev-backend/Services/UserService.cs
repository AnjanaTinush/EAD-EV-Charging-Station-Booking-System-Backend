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

        public Task<List<object>> GetAllAsync() => _userRepository.GetAllAsync();
        public Task<object?> GetByIdAsync(string id) => _userRepository.GetByIdAsync(id);
        public Task<object> CreateAsync(Models.User user) => _userRepository.CreateAsync(user);
        public Task<object?> UpdateAsync(string id, Models.User user) => _userRepository.UpdateAsync(id, user);
        public Task<object?> DeleteAsync(string id) => _userRepository.DeleteAsync(id);
    }
}
