using Ev_backend.DTOs;
using Ev_backend.Models;
using Ev_backend.Models.Enums;
using Ev_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // ========================= REGISTER =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                var createdUser = await _authService.RegisterAsync(user);
                return Ok(new
                {
                    message = "User registered successfully",
                    user = new
                    {
                        id = createdUser.Id,
                        username = createdUser.Username,
                        email = createdUser.Email,
                        phone = createdUser.Phone,
                        nic = createdUser.NIC,
                        role = createdUser.Role.ToString(),
                        isActive = createdUser.IsActive // ✅ added field
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ========================= LOGIN =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto loginDto, [FromQuery] string platform = "web")
        {
            try
            {
                var user = await _authService.LoginAsync(loginDto.NIC, loginDto.Password);

                if (user == null)
                    return Unauthorized(new { message = "Invalid NIC or password" });

                // ✅ Prevent login if account deactivated
                if (!user.IsActive)
                    return StatusCode(403, new { message = "Your account is deactivated. Please contact admin." });

                // ✅ Role-based access control
                switch (platform.ToLower())
                {
                    case "mobile":
                        if (user.Role != UserRole.EvOwner && user.Role != UserRole.StationOperator)
                            return StatusCode(403, new { message = "Your role is not allowed to log in on mobile." });
                        break;

                    case "web":
                        if (user.Role != UserRole.Backoffice && user.Role != UserRole.StationOperator)
                            return StatusCode(403, new { message = "Your role is not allowed to log in on web." });
                        break;
                }

                // ✅ Success response (includes IsActive)
                return Ok(new
                {
                    message = "Login successful",
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        phone = user.Phone,
                        nic = user.NIC,
                        role = user.Role.ToString(),
                        isActive = user.IsActive // ✅ added field here too
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Login failed: {ex.Message}" });
            }
        }
    }
}
