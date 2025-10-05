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
            Console.WriteLine($"🔍 Attempting login with: '{nicOrEmail}', Password length: {password?.Length}");

            // ==================== EV OWNER LOGIN ====================
            // Get all EVOwners and filter in memory for case-insensitive matching
            var allEvOwners = await _evOwners.Find(_ => true).ToListAsync();
            Console.WriteLine($"📊 Total EVOwners in collection: {allEvOwners.Count}");

            var evOwner = allEvOwners.FirstOrDefault(o =>
                o.NIC.Equals(nicOrEmail, StringComparison.OrdinalIgnoreCase) ||
                o.Email.Equals(nicOrEmail, StringComparison.OrdinalIgnoreCase)
            );

            Console.WriteLine($"🔍 EvOwner search result: {(evOwner != null ? $"Found - NIC: {evOwner.NIC}, Email: {evOwner.Email}" : "Not found")}");

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

            Console.WriteLine("EVOwner not found in EvOwners collection");

            // ==================== USERS COLLECTION LOGIN (StationOperator, EvOwner, Backoffice) ====================
            var allUsers = await _users.Find(_ => true).ToListAsync();
            Console.WriteLine($"📊 Total Users in collection: {allUsers.Count}");

            var user = allUsers.FirstOrDefault(u =>
                (u.NIC != null && u.NIC.Equals(nicOrEmail, StringComparison.OrdinalIgnoreCase)) ||
                (u.Email != null && u.Email.Equals(nicOrEmail, StringComparison.OrdinalIgnoreCase))
            );

            Console.WriteLine($"🔍 User search result: {(user != null ? $"Found - Role: {user.Role}" : "Not found")}");

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
