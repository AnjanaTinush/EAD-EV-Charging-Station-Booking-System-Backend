using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IMongoDatabase _db;

        public HealthController(IMongoDatabase db)
        {
            _db = db;
        }

        [HttpGet("db-check")]
        public async Task<IActionResult> CheckDb()
        {
            try
            {
                await _db.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                return Ok("✅ MongoDB connected successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ MongoDB connection failed: {ex.Message}");
            }
        }
    }
}
