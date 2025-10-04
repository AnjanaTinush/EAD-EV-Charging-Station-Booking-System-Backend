using Microsoft.AspNetCore.Mvc;
using Ev_backend.Services;
using System.Threading.Tasks;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MobileAuthController : ControllerBase
    {
        private readonly MobileAuth _authService;

        public MobileAuthController(MobileAuth authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.AuthenticateAsync(request.NIC, request.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid NIC, password or role" });

            return Ok(new { message = "Login successful", user });
        }
    }

    public class LoginRequest
    {
        public string NIC { get; set; }
        public string Password { get; set; }
    }
}
