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

        public async Task<List<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            // Default password
            user.Password = "000000";
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(string id, User userIn)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            // Don't update password
            var update = Builders<User>.Update
                .Set(u => u.Username, userIn.Username)
                .Set(u => u.Email, userIn.Email)
                .Set(u => u.Phone, userIn.Phone)
                .Set(u => u.Role, userIn.Role);
            await _users.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            await _users.DeleteOneAsync(filter);
        }
    }
}