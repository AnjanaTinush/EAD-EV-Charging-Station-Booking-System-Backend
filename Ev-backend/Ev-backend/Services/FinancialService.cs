using Ev_backend.Dtos;
using Ev_backend.Models;
using Ev_backend.Repositories;
using MongoDB.Driver;

namespace Ev_backend.Services
{
    public class FinancialService
    {
        private readonly FinancialRepository _repository;

        public FinancialService(FinancialRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Financial>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Financial?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

        public async Task<Financial> CreateAsync(Financial financial)
        {
            if (string.IsNullOrWhiteSpace(financial.Username))
                throw new Exception("Username is required.");

            if (string.IsNullOrWhiteSpace(financial.NIC))
                throw new Exception("NIC is required.");

            financial.Id = null;
            financial.Status = "Pending";

            await _repository.CreateAsync(financial);
            return financial;
        }

        public async Task<Financial> UpdateAsync(string id, Financial financial)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Financial record not found.");

            financial.Id = id;
            await _repository.UpdateAsync(id, financial);
            return financial;
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Financial record not found.");

            await _repository.DeleteAsync(id);
        }

        public async Task<Financial> PatchAsync(string id, FinancialPatchDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Financial record not found.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new Exception("Status value is required.");

            var updateDef = Builders<Financial>.Update.Set(f => f.Status, dto.Status);
            await _repository.UpdateFieldsAsync(id, updateDef);

            existing.Status = dto.Status;
            return existing;
        }
    }
}
