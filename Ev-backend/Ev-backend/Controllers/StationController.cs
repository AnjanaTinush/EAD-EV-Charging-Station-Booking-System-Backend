using Microsoft.AspNetCore.Mvc;
using Ev_backend.Models;
using Ev_backend.Services;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationController : ControllerBase
    {
        private readonly StationService _service;

        public StationController(StationService service)
        {
            _service = service;
        }

        // GET /api/station
        [HttpGet]
        public async Task<ActionResult<List<Station>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        // GET /api/station/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Station>> Get(string id)
        {
            var station = await _service.GetByIdAsync(id);
            return station == null ? NotFound() : Ok(station);
        }

        // POST /api/station
        [HttpPost]
        public async Task<ActionResult<Station>> Create(Station station)
        {
            try
            {
                // 👇 Always reset Id so MongoDB generates a new one
                station.Id = null;

                var created = await _service.CreateAsync(station);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // PUT /api/station/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Station station)
        {
            try
            {
                await _service.UpdateAsync(id, station);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/station/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
