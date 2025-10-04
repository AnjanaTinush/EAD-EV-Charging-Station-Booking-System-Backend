using Ev_backend.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Ev_backend.Services
{
    public class MobileAuth
    {
        private readonly IMongoCollection<User> _users;

        public MobileAuth(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users"); // Mongo collection name
        }

        /// <summary>
        /// Authenticate only StationOperator & EvOwner users using NIC + Password
        /// </summary>
        public async Task<User?> AuthenticateAsync(string nic, string password)
        {
            // find user by NIC
            var user = await _users.Find(u => u.NIC == nic).FirstOrDefaultAsync();

            if (user == null) return null;

            // check allowed roles
            if (user.Role != UserRole.StationOperator && user.Role != UserRole.EvOwner)
                return null;

            // password check (plain text for now - better: hash + salt)
            if (user.Password != password)
                return null;

            // check active status
            if (!user.IsActive)
                return null;

            return user;
        }
    }
}
