using Ev_backend.DTOs;
using Ev_backend.Models;
using Ev_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // ✅ Get all users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // ✅ Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // ✅ Create new user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                NIC = dto.NIC,
                Password = "000000",
                Role = dto.Role ?? UserRole.Backoffice
            };

            await _userService.CreateAsync(user);
            return Ok(new { message = "User created successfully", user });
        }

        // ✅ Update user (keeps password)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "User not found" });

            var updatedUser = new User
            {
                Id = existing.Id,
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                NIC = dto.NIC,
                Password = existing.Password,
                Role = dto.Role,
                IsActive = existing.IsActive // 👈 keep isActive as it is
            };

            await _userService.UpdateAsync(id, updatedUser);
            return Ok(new { message = "User updated successfully", updatedUser });
        }

        // ✅ Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "User not found" });

            await _userService.DeleteAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }

        // ✅ DEACTIVATE user
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "User not found" });

            if (!existing.IsActive)
                return BadRequest(new { message = "User is already deactivated" });

            await _userService.DeactivateAsync(id);
            return Ok(new { message = "User account deactivated successfully" });
        }

        // ✅ ACTIVATE user
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "User not found" });

            if (existing.IsActive)
                return BadRequest(new { message = "User is already active" });

            await _userService.ActivateAsync(id);
            return Ok(new { message = "User account activated successfully" });
        }

        // ✅ Get user by NIC
        [HttpGet("by-nic/{nic}")]
        public async Task<IActionResult> GetByNIC(string nic)
        {
            var user = await _userService.GetByNICAsync(nic);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        // ✅ Get user by Email
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
    }
}
