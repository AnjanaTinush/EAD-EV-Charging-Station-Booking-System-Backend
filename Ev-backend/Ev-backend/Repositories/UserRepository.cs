using Ev_backend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<BsonDocument> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<BsonDocument>("Users");
        }

        // ===== Helper: Extract Role safely (works for string or int) =====
        private string GetRole(BsonDocument doc)
        {
            if (!doc.Contains("role") || doc["role"].IsBsonNull)
                return null;

            return doc["role"].BsonType switch
            {
                BsonType.String => doc["role"].AsString,
                BsonType.Int32 => Enum.GetName(typeof(UserRole), doc["role"].AsInt32),
                BsonType.Int64 => Enum.GetName(typeof(UserRole), (int)doc["role"].AsInt64),
                _ => doc["role"].ToString()
            };
        }

        // ========== Get All ==========
        public async Task<List<object>> GetAllAsync()
        {
            var docs = await _users.Find(_ => true).ToListAsync();

            return docs.Select(d => new
            {
                Id = d["_id"].ToString(),
                Username = d.Contains("username") ? d["username"].AsString : null,
                Email = d.Contains("email") ? d["email"].AsString : null,
                Phone = d.Contains("phone") ? d["phone"].AsString : null,
                Role = GetRole(d)
            } as object).ToList();
        }

        // ========== Get By Id ==========
        public async Task<object?> GetByIdAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            var doc = await _users.Find(filter).FirstOrDefaultAsync();
            if (doc == null) return null;

            return new
            {
                Id = doc["_id"].ToString(),
                Username = doc.Contains("username") ? doc["username"].AsString : null,
                Email = doc.Contains("email") ? doc["email"].AsString : null,
                Phone = doc.Contains("phone") ? doc["phone"].AsString : null,
                Role = GetRole(doc)
            };
        }

        // ========== Create ==========
        public async Task<object> CreateAsync(User user)
        {
            var doc = user.ToBsonDocument();
            doc["password"] = "000000"; // default password
            await _users.InsertOneAsync(doc);

            return new
            {
                Id = doc["_id"].ToString(),
                Username = doc.Contains("username") ? doc["username"].AsString : null,
                Email = doc.Contains("email") ? doc["email"].AsString : null,
                Phone = doc.Contains("phone") ? doc["phone"].AsString : null,
                Role = GetRole(doc),
                message = "User created successfully with default password 000000"
            };
        }

        // ========== Update ==========
        public async Task<object?> UpdateAsync(string id, User userIn)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var update = Builders<BsonDocument>.Update
                .Set("username", userIn.Username)
                .Set("email", userIn.Email)
                .Set("phone", userIn.Phone)
                .Set("role", userIn.Role.ToString()); // store role as string

            var result = await _users.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0) return null;

            return new
            {
                Id = id,
                userIn.Username,
                userIn.Email,
                userIn.Phone,
                Role = userIn.Role.ToString(),
                message = "User updated successfully (password unchanged)"
            };
        }

        // ========== Delete ==========
        public async Task<object?> DeleteAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = await _users.DeleteOneAsync(filter);

            if (result.DeletedCount == 0) return null;

            return new { Id = id, message = "User deleted successfully" };
        }
    }
}
