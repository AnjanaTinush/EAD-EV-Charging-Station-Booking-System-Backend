using Ev_backend.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Ev_backend.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _users.Find(u => u.Phone == phone).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        // fetch by MongoDB ObjectId
        public async Task<User?> GetByObjectIdAsync(string objectId)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(objectId));
            return await _users.Find(filter).FirstOrDefaultAsync();
        }
    }
}
