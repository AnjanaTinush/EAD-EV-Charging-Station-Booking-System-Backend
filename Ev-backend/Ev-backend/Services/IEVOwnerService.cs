using Ev_backend.DTOs;

namespace Ev_backend.Services
{
    public interface IEVOwnerService
    {
        Task<EVOwnerResponseDto> CreateAsync(CreateEVOwnerDto dto);
        Task<EVOwnerResponseDto> UpdateAsync(string id, UpdateEVOwnerDto dto);
        Task<EVOwnerResponseDto?> GetByIdAsync(string id);
        Task<EVOwnerResponseDto?> GetByNICAsync(string nic);
        Task<List<EVOwnerResponseDto>> GetAllAsync();
        Task DeleteAsync(string id);
    }
}
