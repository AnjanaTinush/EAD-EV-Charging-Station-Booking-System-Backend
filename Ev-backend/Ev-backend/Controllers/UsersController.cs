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
                IsActive = true
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
                    Password = "(hashed)"
                }
            });
        }

        // ✅ Update user (role optional)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] JsonElement jsonBody)
        {
            // ✅ Allow case-insensitive property matching
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var dto = JsonSerializer.Deserialize<UserUpdateDto>(jsonBody, options);

            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "User not found" });

            if (dto == null)
                return BadRequest(new { message = "Invalid request body" });

            // ✅ Validate phone (required)
            if (string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest(new { message = "Phone number is required." });

            if (!PhoneRegex().IsMatch(dto.Phone))
                return BadRequest(new { message = "Phone number must be exactly 10 digits." });

            // ✅ Handle NIC (optional)
            string newNic = existing.NIC;
            if (jsonBody.TryGetProperty("nic", out _))
            {
                if (string.IsNullOrWhiteSpace(dto.NIC))
                    return BadRequest(new { message = "NIC cannot be empty if provided." });

                if (!NicRegex().IsMatch(dto.NIC))
                    return BadRequest(new { message = "NIC must be exactly 12 digits." });

                if (dto.NIC != existing.NIC)
                {
                    var nicExists = await _userService.ExistsByNICAsync(dto.NIC);
                    if (nicExists)
                        return Conflict(new { message = $"Another user with NIC '{dto.NIC}' already exists." });
                }

                newNic = dto.NIC;
            }

            // ✅ Check email uniqueness (if provided)
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var emailExists = await _userService.ExistsByEmailAsync(dto.Email);
                if (emailExists && dto.Email != existing.Email)
                    return Conflict(new { message = $"Another user with email '{dto.Email}' already exists." });
            }

            // ✅ Build updated user object
            var updatedUser = new User
            {
                Id = existing.Id,
                Username = dto.Username ?? existing.Username,
                Email = dto.Email ?? existing.Email,
                Phone = dto.Phone,
                NIC = newNic,
                Password = existing.Password,
                IsActive = existing.IsActive,
                Role = jsonBody.TryGetProperty("role", out _)
                    ? dto.Role ?? existing.Role
                    : existing.Role
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

        // ✅ Deactivate / Activate
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

        // ✅ Get by NIC / Email
        [HttpGet("by-nic/{nic}")]
        public async Task<IActionResult> GetByNIC(string nic)
        {
            var user = await _userService.GetByNICAsync(nic);
            return user == null ? NotFound(new { message = "User not found" }) : Ok(user);
        }

        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            return user == null ? NotFound(new { message = "User not found" }) : Ok(user);
        }

        // ✅ Compile-time regexes
        [GeneratedRegex("^\\d{12}$")]
        private static partial Regex NicRegex();

        [GeneratedRegex("^\\d{10}$")]
        private static partial Regex PhoneRegex();
    }
}
