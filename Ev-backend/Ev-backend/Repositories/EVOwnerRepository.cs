using Ev_backend.Config;
using Ev_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class EVOwnerRepository : IEVOwnerRepository
    {
        private readonly IMongoCollection<EVOwner> _collection;

        public EVOwnerRepository(IOptions<MongoDbSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.DatabaseName);
            _collection = db.GetCollection<EVOwner>("evowners");
        }

        public Task<EVOwner> GetByIdAsync(string id) =>
            _collection.Find(o => o.Id == id).FirstOrDefaultAsync();

        public Task<EVOwner> GetByNICAsync(string nic) =>
            _collection.Find(o => o.NIC == nic).FirstOrDefaultAsync();

        public Task<List<EVOwner>> GetAllAsync() =>
            _collection.Find(_ => true).ToListAsync();

        public async Task<EVOwner> InsertAsync(EVOwner owner)
        {
            await _collection.InsertOneAsync(owner);
            return owner;
        }

        public Task UpdateAsync(EVOwner owner) =>
            _collection.ReplaceOneAsync(o => o.Id == owner.Id, owner);

        public Task DeleteAsync(string id) =>
            _collection.DeleteOneAsync(o => o.Id == id);
    }
}
