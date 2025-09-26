using Ev_backend.Models;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class AuthRepository
    {
        private readonly IMongoCollection<User> _users;

        public AuthRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        /// <summary>
        /// Get user by email (used for login and register validation).
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        /// <summary>
        /// Validate login by email and password.
        /// </summary>
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            return await _users.Find(u => u.Email == email && u.Password == password)
                               .FirstOrDefaultAsync();
        }
    }
}
