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

        // ✅ Get all users
        public async Task<List<User>> GetAllAsync() =>
            await _users.Find(_ => true).ToListAsync();

        // ✅ Get user by ID (safe ObjectId parsing)
        public async Task<User?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return null;

            var filter = Builders<User>.Filter.Eq("_id", objectId);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        // ✅ Create new user (auto sets timestamps)
        public async Task CreateAsync(User user)
        {
            // Default password (hashed version should be set in service/controller)
            if (string.IsNullOrEmpty(user.Password))
                user.Password = "000000";

            // Default role
            if (!Enum.IsDefined(typeof(UserRole), user.Role))
                user.Role = UserRole.Backoffice;

            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _users.InsertOneAsync(user);
        }

        // ✅ Update user (auto updates UpdatedAt)
        public async Task UpdateAsync(string id, User userIn)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return;

            var filter = Builders<User>.Filter.Eq("_id", objectId);

            var update = Builders<User>.Update
                .Set(u => u.Username, userIn.Username)
                .Set(u => u.Email, userIn.Email)
                .Set(u => u.Phone, userIn.Phone)
                .Set(u => u.NIC, userIn.NIC)
                .Set(u => u.Role, userIn.Role)
                .Set(u => u.IsActive, userIn.IsActive)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _users.UpdateOneAsync(filter, update);
        }

        // ✅ Delete user
        public async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return;

            var filter = Builders<User>.Filter.Eq("_id", objectId);
            await _users.DeleteOneAsync(filter);
        }

        // ========================= STATUS UPDATE =========================

        // ✅ Activate / Deactivate user (auto updates UpdatedAt)
        public async Task SetActiveStatusAsync(string id, bool isActive)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return;

            var filter = Builders<User>.Filter.Eq("_id", objectId);
            var update = Builders<User>.Update
                .Set(u => u.IsActive, isActive)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);

            await _users.UpdateOneAsync(filter, update);
        }

        // ========================= HELPER QUERIES =========================

        // ✅ Get by NIC
        public async Task<User?> GetByNICAsync(string nic)
        {
            var filter = Builders<User>.Filter.Eq(u => u.NIC, nic);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        // ✅ Get by Email
        public async Task<User?> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        // ✅ Check if NIC exists
        public async Task<bool> ExistsByNICAsync(string nic)
        {
            var filter = Builders<User>.Filter.Eq(u => u.NIC, nic);
            return await _users.CountDocumentsAsync(filter) > 0;
        }

        // ✅ Check if Email exists
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _users.CountDocumentsAsync(filter) > 0;
        }
    }
}
