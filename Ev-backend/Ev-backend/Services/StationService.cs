using Ev_backend.Models;
using Ev_backend.Repositories;

namespace Ev_backend.Services
{
    public class StationService
    {
        private readonly StationRepository _repository;

        public StationService(StationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Station>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<Station?> GetByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task<Station> CreateAsync(Station station)
        {
            if (string.IsNullOrWhiteSpace(station.Name))
                throw new Exception("Station must have a name.");

            await _repository.CreateAsync(station);
            return station;
        }

        public async Task UpdateAsync(string id, Station station)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Station not found.");

            station.Id = id; // preserve same ID
            await _repository.UpdateAsync(id, station);
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Station not found.");

            if (existing.IsActive)
                throw new Exception("Cannot delete an active station. Please deactivate it first.");

            await _repository.DeleteAsync(id);
        }
    }
}
