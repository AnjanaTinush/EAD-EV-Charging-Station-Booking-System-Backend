using Ev_backend.Models;

namespace Ev_backend.Repositories
{
    public interface IEVOwnerRepository
    {
        Task<EVOwner?> GetByIdAsync(string id);
        Task<EVOwner?> GetByNICAsync(string nic);
        Task<List<EVOwner>> GetAllAsync();
        Task<EVOwner> InsertAsync(EVOwner owner);
        Task UpdateAsync(EVOwner owner);
        Task DeleteAsync(string id);
    }
}
