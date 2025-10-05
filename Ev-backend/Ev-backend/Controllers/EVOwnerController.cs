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
            if (owner == null) return NotFound(new { message = "Owner not found" });
            return Ok(owner);
        }

        [HttpGet("by-nic/{nic}")]
        public async Task<ActionResult<EVOwnerResponseDto>> GetByNIC(string nic)
        {
            var owner = await _service.GetByNICAsync(nic);
            if (owner == null) return NotFound(new { message = "Owner not found" });
            return Ok(owner);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEVOwnerDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                // Duplicate NIC or validation issue
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Other unexpected error
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateEVOwnerDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Owner not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}
