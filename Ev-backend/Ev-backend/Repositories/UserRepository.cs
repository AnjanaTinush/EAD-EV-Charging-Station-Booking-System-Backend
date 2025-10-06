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

        // ========================= BASIC CRUD =========================

        public async Task<List<User>> GetAllAsync() =>
            await _users.Find(_ => true).ToListAsync();

        public async Task<User?> GetByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            user.Password = "000000";
            if (!Enum.IsDefined(typeof(UserRole), user.Role) || user.Role == 0)
                user.Role = UserRole.Backoffice;

            await _users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(string id, User userIn)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<User>.Update
                .Set(u => u.Username, userIn.Username)
                .Set(u => u.Email, userIn.Email)
                .Set(u => u.Phone, userIn.Phone)
                .Set(u => u.NIC, userIn.NIC)
                .Set(u => u.Role, userIn.Role)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _users.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            await _users.DeleteOneAsync(filter);
        }

        // ========================= STATUS UPDATE =========================

        // ✅ Unified method to activate or deactivate without touching other fields
        public async Task SetActiveStatusAsync(string id, bool isActive)
        {
            var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<User>.Update
                .Set(u => u.IsActive, isActive)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _users.UpdateOneAsync(filter, update);
        }

        // ========================= OTHER HELPERS =========================

        public async Task<User?> GetByNICAsync(string nic)
        {
            var filter = Builders<User>.Filter.Eq(u => u.NIC, nic);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByNICAsync(string nic)
        {
            var filter = Builders<User>.Filter.Eq(u => u.NIC, nic);
            var count = await _users.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var count = await _users.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}
