using BCrypt.Net;
using Ev_backend.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Ev_backend.Services
{
    public class MobileAuth
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<EVOwner> _evOwners;

        public MobileAuth(IMongoDatabase database)
        {
            // ✅ Must match MongoDB Compass casing
            _users = database.GetCollection<User>("Users");
            _evOwners = database.GetCollection<EVOwner>("EvOwners");
        }

        /// <summary>
        /// Authenticates both EV Owners (by NIC/Email) and Station Operators (by NIC).
        /// </summary>
        public async Task<object?> AuthenticateAsync(string nicOrEmail, string password)
        {
            // ==================== EV OWNER LOGIN ====================
            var evOwnerFilter = Builders<EVOwner>.Filter.Or(
               Builders<EVOwner>.Filter.Regex("nic", new BsonRegularExpression($"^{nicOrEmail}$", "i")),
               Builders<EVOwner>.Filter.Regex("email", new BsonRegularExpression($"^{nicOrEmail}$", "i"))
           );
            var evOwner = await _evOwners.Find(evOwnerFilter).FirstOrDefaultAsync();

            if (evOwner != null)
            {
                // DEBUG: Log that we found the owner
                Console.WriteLine($"Found EVOwner: {evOwner.Id}, IsActive: {evOwner.IsActive}");

                if (!evOwner.IsActive)
                {
                    Console.WriteLine("EVOwner is not active");
                    return null;
                }

                bool passwordMatch;
                try
                {
                    passwordMatch = BCrypt.Net.BCrypt.Verify(password, evOwner.Password);
                    Console.WriteLine($"BCrypt verify result: {passwordMatch}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BCrypt failed: {ex.Message}");
                    // fallback for older plain passwords
                    passwordMatch = evOwner.Password == password;
                    Console.WriteLine($"Plain text verify result: {passwordMatch}");
                }

                if (passwordMatch)
                {
                    return new
                    {
                        id = evOwner.Id,
                        email = evOwner.Email,
                        username = evOwner.FullName,
                        role = "EVOwner"
                    };
                }

                Console.WriteLine("Password did not match");
                return null;
            }

            Console.WriteLine("EVOwner not found in database");

            // ==================== STATION OPERATOR LOGIN ====================
            // ... rest of your code
        

        // ==================== STATION OPERATOR LOGIN ====================
        var user = await _users
                .Find(u => u.NIC == nicOrEmail && u.Role == UserRole.StationOperator)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                if (!user.IsActive)
                    return null;

                bool passwordMatch;
                try
                {
                    passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
                }
                catch
                {
                    passwordMatch = user.Password == password;
                }

                if (passwordMatch)
                {
                    return new
                    {
                        id = user.Id,
                        email = user.Email,
                        username = user.Username,
                        role = user.Role.ToString()
                    };
                }

                // Wrong password
                return null;
            }

            // ==================== NOT FOUND IN EITHER COLLECTION ====================
            return null;
        }
    }
}
