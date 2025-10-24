using Ev_backend.Models;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class FinancialRepository
    {
        private readonly IMongoCollection<Financial> _financials;

        public FinancialRepository(IMongoDatabase database)
        {
            _financials = database.GetCollection<Financial>("Financials");
        }

        public async Task<List<Financial>> GetAllAsync() =>
            await _financials.Find(_ => true).ToListAsync();

        public async Task<Financial?> GetByIdAsync(string id) =>
            await _financials.Find(f => f.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Financial financial) =>
            await _financials.InsertOneAsync(financial);

        public async Task UpdateAsync(string id, Financial financial) =>
            await _financials.ReplaceOneAsync(f => f.Id == id, financial);

        public async Task DeleteAsync(string id) =>
            await _financials.DeleteOneAsync(f => f.Id == id);

        public async Task UpdateFieldsAsync(string id, UpdateDefinition<Financial> updateDef)
        {
            await _financials.UpdateOneAsync(f => f.Id == id, updateDef);
        }
    }
}
