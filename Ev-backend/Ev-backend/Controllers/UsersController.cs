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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null) return NotFound(new { message = "User not found" });
                return Ok(user);
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid user ID format" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Invalid user data" });

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Role = dto.Role,
                    Password = "000000" // 👈 default password
                };

                await _userService.CreateAsync(user);
                return Ok(new { message = "User created successfully with default password 000000" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] User user)
        {
            try
            {
                var existing = await _userService.GetByIdAsync(id);
                if (existing == null) return NotFound(new { message = "User not found" });

                await _userService.UpdateAsync(id, user);
                return Ok(new { message = "User updated successfully (password unchanged)" });
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid user ID format" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existing = await _userService.GetByIdAsync(id);
                if (existing == null) return NotFound(new { message = "User not found" });

                await _userService.DeleteAsync(id);
                return Ok(new { message = "User deleted successfully" });
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid user ID format" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}