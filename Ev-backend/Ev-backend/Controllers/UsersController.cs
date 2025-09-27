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
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid user data" });

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                Password = "000000" // Default password
            };

            var result = await _userService.CreateAsync(user);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid user data" });

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role
                // 👈 Notice: No Password here
            };

            var result = await _userService.UpdateAsync(id, user);
            if (result == null) return NotFound(new { message = "User not found" });
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);
            if (result == null) return NotFound(new { message = "User not found" });
            return Ok(result);
        }
    }
}
