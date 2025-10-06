using Ev_backend.DTOs;
using Ev_backend.Models;
using Ev_backend.Repositories;
using BCrypt.Net;

namespace Ev_backend.Services
{
    public class EVOwnerService : IEVOwnerService
    {
        private readonly IEVOwnerRepository _repo;

        public EVOwnerService(IEVOwnerRepository repo)
        {
            _repo = repo;
        }

        public async Task<EVOwnerResponseDto> CreateAsync(CreateEVOwnerDto dto)
        {
            // ✅ Step 1: Check if NIC already exists
            var existingOwner = await _repo.GetByNICAsync(dto.NIC);
            if (existingOwner != null)
            {
                throw new InvalidOperationException($"An owner with NIC '{dto.NIC}' already exists.");
            }

            // ✅ Step 2: Create new owner with default hashed password
            var entity = new EVOwner
            {
                NIC = dto.NIC,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                IsActive = true,
                Password = BCrypt.Net.BCrypt.HashPassword("000000")
            };

            entity = await _repo.InsertAsync(entity);
            return Map(entity);
        }

        public async Task<EVOwnerResponseDto> UpdateAsync(string id, UpdateEVOwnerDto dto)
        {
            var owner = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Owner not found");

            owner.FullName = dto.FullName;
            owner.Email = dto.Email;
            owner.Phone = dto.Phone;
            owner.IsActive = dto.IsActive;

            // keep existing password; set default if missing
            if (string.IsNullOrEmpty(owner.Password))
                owner.Password = BCrypt.Net.BCrypt.HashPassword("000000");

            await _repo.UpdateAsync(owner);
            return Map(owner);
        }

        public async Task<EVOwnerResponseDto?> GetByIdAsync(string id)
        {
            var owner = await _repo.GetByIdAsync(id);
            return owner is null ? null : Map(owner);
        }

        public async Task<EVOwnerResponseDto?> GetByNICAsync(string nic)
        {
            var owner = await _repo.GetByNICAsync(nic);
            return owner is null ? null : Map(owner);
        }

        public async Task<List<EVOwnerResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(Map).ToList();
        }

        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);

        private static EVOwnerResponseDto Map(EVOwner e) => new()
        {
            Id = e.Id,
            NIC = e.NIC,
            FullName = e.FullName,
            Email = e.Email,
            Phone = e.Phone,
            IsActive = e.IsActive
        };
    }
}
