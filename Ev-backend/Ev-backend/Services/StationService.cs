using Ev_backend.Dtos;
using Ev_backend.Models;
using Ev_backend.Repositories;
using MongoDB.Driver;

namespace Ev_backend.Services
{
    public class StationService
    {
        private readonly StationRepository _repository;

        public StationService(StationRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Station>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Station?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

        public async Task<Station> CreateAsync(Station station)
        {
            if (string.IsNullOrWhiteSpace(station.Name))
                throw new Exception("Station must have a name.");

            // let Mongo generate Id
            station.Id = null;
            await _repository.CreateAsync(station);
            return station;
        }

        public async Task UpdateAsync(string id, Station station)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Station not found.");

            station.Id = id; // preserve ID
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

        // Partial update using a typed DTO
        public async Task PatchAsync(string id, StationPatchDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Station not found.");

            var updates = new List<UpdateDefinition<Station>>();

            if (dto.IsActive.HasValue)
                updates.Add(Builders<Station>.Update.Set(s => s.IsActive, dto.IsActive.Value));

            if (dto.AvailableSlots.HasValue)
                updates.Add(Builders<Station>.Update.Set(s => s.AvailableSlots, dto.AvailableSlots.Value));

            if (updates.Count == 0)
                throw new Exception("No valid fields provided to update.");

            var updateDef = Builders<Station>.Update.Combine(updates);
            await _repository.UpdateFieldsAsync(id, updateDef);
        }
    }
}
