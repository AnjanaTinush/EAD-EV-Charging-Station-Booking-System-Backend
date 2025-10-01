using Ev_backend.DTOs;
using Ev_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EVOwnerController : ControllerBase
    {
        private readonly IEVOwnerService _service;

        public EVOwnerController(IEVOwnerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<EVOwnerResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EVOwnerResponseDto>> GetById(string id)
        {
            var owner = await _service.GetByIdAsync(id);
            if (owner == null) return NotFound();
            return Ok(owner);
        }

        [HttpGet("by-nic/{nic}")]
        public async Task<ActionResult<EVOwnerResponseDto>> GetByNIC(string nic)
        {
            var owner = await _service.GetByNICAsync(nic);
            if (owner == null) return NotFound();
            return Ok(owner);
        }

        [HttpPost]
        public async Task<ActionResult<EVOwnerResponseDto>> Create([FromBody] CreateEVOwnerDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EVOwnerResponseDto>> Update(string id, [FromBody] UpdateEVOwnerDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
