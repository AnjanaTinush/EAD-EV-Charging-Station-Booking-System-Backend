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
            _database = client.GetDatabase("EVChargingDB"); // 👈 use your DB name
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                var createdUser = await _authService.RegisterAsync(user);

                var usersCollection = _database.GetCollection<BsonDocument>("Users");
                var filter = Builders<BsonDocument>.Filter.Eq("username", createdUser.Username);
                var bsonUser = await usersCollection.Find(filter).FirstOrDefaultAsync();

                return Ok(new
                {
                    message = "User registered successfully",
                    user = new
                    {
                        id = bsonUser["_id"].ToString(),
                        username = createdUser.Username,
                        phone = createdUser.Phone,
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
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _authService.LoginAsync(loginDto.Username, loginDto.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var usersCollection = _database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", user.Username);
            var bsonUser = await usersCollection.Find(filter).FirstOrDefaultAsync();

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    id = bsonUser["_id"].ToString(),
                    username = user.Username,
                    phone = user.Phone,
                    role = user.Role
                }
            });
        }
    }
}
