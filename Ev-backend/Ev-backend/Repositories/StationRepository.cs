using Ev_backend.Models;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class StationRepository
    {
        private readonly IMongoCollection<Station> _stations;

        public StationRepository(IMongoDatabase database)
        {
            _stations = database.GetCollection<Station>("Stations");
        }

        public async Task<List<Station>> GetAllAsync() =>
            await _stations.Find(_ => true).ToListAsync();

        public async Task<Station?> GetByIdAsync(string id) =>
            await _stations.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Station station) =>
            await _stations.InsertOneAsync(station);

        public async Task UpdateAsync(string id, Station station) =>
            await _stations.ReplaceOneAsync(s => s.Id == id, station);

        public async Task DeleteAsync(string id) =>
            await _stations.DeleteOneAsync(s => s.Id == id);
    }
}
