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

        // ✅ Get user by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        // ✅ Create new user (default password = 000000, role = Backoffice if not provided)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                NIC = dto.NIC,
                Password = "000000", // default password
                Role = dto.Role ?? UserRole.Backoffice // default role
            };

            await _userService.CreateAsync(user);
            return Ok(new { message = "User created successfully", user });
        }

        // ✅ Update user (password not updated here)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "User not found" });

            var updatedUser = new User
            {
                Id = existing.Id,
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                NIC = dto.NIC,
                Password = existing.Password, // keep old password
                Role = dto.Role
            };

            await _userService.UpdateAsync(id, updatedUser);
            return Ok(new { message = "User updated successfully", updatedUser });
        }

        // ✅ Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "User not found" });

            await _userService.DeleteAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }

        // ✅ Deactivate user account
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "User not found" });

            await _userService.DeactivateAsync(id);
            return Ok(new { message = "User account deactivated successfully" });
        }

        // ✅ Reactivate user account (Backoffice only)
        [HttpPatch("{id}/reactivate")]
        public async Task<IActionResult> Reactivate(string id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "User not found" });

            if (existing.IsActive) return BadRequest(new { message = "User account is already active" });

            await _userService.ReactivateAsync(id);
            return Ok(new { message = "User account reactivated successfully" });
        }

        // ✅ Get user by NIC
        [HttpGet("by-nic/{nic}")]
        public async Task<IActionResult> GetByNIC(string nic)
        {
            var user = await _userService.GetByNICAsync(nic);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        // ✅ Get user by Email
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }
    }
}
