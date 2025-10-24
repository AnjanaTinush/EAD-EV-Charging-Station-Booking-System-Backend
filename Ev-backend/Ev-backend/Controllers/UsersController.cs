using BCrypt.Net;
using Ev_backend.DTOs;
using Ev_backend.Models;
using Ev_backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class UsersController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

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
            if (string.IsNullOrWhiteSpace(dto.NIC))
                return BadRequest(new { message = "NIC is required." });
            if (string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest(new { message = "Phone number is required." });

            if (!NicRegex().IsMatch(dto.NIC))
                return BadRequest(new { message = "NIC must be exactly 12 digits." });
            if (!PhoneRegex().IsMatch(dto.Phone))
                return BadRequest(new { message = "Phone number must be exactly 10 digits." });

            if (await _userService.ExistsByNICAsync(dto.NIC))
                return Conflict(new { message = $"A user with NIC '{dto.NIC}' already exists." });

            if (!string.IsNullOrEmpty(dto.Email) &&
                await _userService.ExistsByEmailAsync(dto.Email))
                return Conflict(new { message = $"A user with email '{dto.Email}' already exists." });

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("000000");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                NIC = dto.NIC,
                Password = hashedPassword,
                Role = dto.Role ?? UserRole.Backoffice,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);

            return Ok(new
            {
                message = "User created successfully",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Phone,
                    user.NIC,
                    user.Role,
                    Password = "(hashed)",
                    user.CreatedAt,
                    user.UpdatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "User not found" });

            // ✅ Role update now works
            var newRole = dto.Role ?? existing.Role;

            var updatedUser = new User
            {
                Id = existing.Id,
                Username = dto.Username ?? existing.Username,
                Email = dto.Email ?? existing.Email,
                Phone = dto.Phone,
                NIC = dto.NIC ?? existing.NIC,
                Password = existing.Password,
                Role = newRole,
                IsActive = existing.IsActive,
                UpdatedAt = DateTime.UtcNow
            };

            await _userService.UpdateAsync(id, updatedUser);
            var refreshedUser = await _userService.GetByIdAsync(id);

            return Ok(new
            {
                message = "User updated successfully",
                updatedUser = refreshedUser
            });
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

        // ✅ Activate / Deactivate
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

        // ✅ Compile-time regexes
        [GeneratedRegex("^\\d{12}$")]
        private static partial Regex NicRegex();

        [GeneratedRegex("^\\d{10}$")]
        private static partial Regex PhoneRegex();
    }
}
