using Ev_backend.DTOs;
using Ev_backend.Models;
using Ev_backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMongoDatabase _database;

        public AuthController(AuthService authService, IMongoClient client)
        {
            _authService = authService;
            _database = client.GetDatabase("EVChargingDB");
        }

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
                        role = createdUser.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto loginDto)
        {
            var user = await _authService.LoginAsync(loginDto.NIC, loginDto.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid NIC or password" });

            var usersCollection = _database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("nic", user.NIC);
            var bsonUser = await usersCollection.Find(filter).FirstOrDefaultAsync();

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    id = bsonUser["_id"].ToString(),
                    username = user.Username,
                    email = user.Email,
                    phone = user.Phone,
                    nic = user.NIC,
                    role = user.Role
                }
            });
        }
    }
}
