using Ev_backend.Models;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class AuthRepository
    {
        private readonly IMongoCollection<User> _users;

        public AuthRepository(IMongoDatabase database)
        {
            // Use the correct collection name in MongoDB (case-sensitive)
            _users = database.GetCollection<User>("Users");
        }

        // ✅ Get user by Email
        public async Task<User?> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        // ✅ Get user by NIC
        public async Task<User?> GetByNICAsync(string nic) =>
            await _users.Find(u => u.NIC == nic).FirstOrDefaultAsync();

        // ✅ Create a new user
        public async Task CreateAsync(User user) =>
            await _users.InsertOneAsync(user);
    }
}
