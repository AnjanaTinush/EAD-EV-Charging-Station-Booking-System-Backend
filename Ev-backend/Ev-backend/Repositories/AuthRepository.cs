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

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByOwnerNICAsync(string nic)
        {
            return await _users.Find(u => u.NIC == nic).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        /// <summary>
        /// Validate login by NIC and Password.
        /// </summary>
        public async Task<User?> ValidateUserAsync(string nic, string password)
        {
            return await _users.Find(u => u.NIC == nic && u.Password == password)
                               .FirstOrDefaultAsync();
        }
    }
}
