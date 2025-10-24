using Microsoft.AspNetCore.Mvc;
using Ev_backend.Models;
using Ev_backend.Services;
using Ev_backend.Dtos;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly FinancialService _service;

        public FinancialController(FinancialService service)
        {
            _service = service;
        }

        // GET all
        [HttpGet]
        public async Task<ActionResult<List<Financial>>> GetAll()
            => Ok(await _service.GetAllAsync());

        // GET by ID
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Financial>> Get(string id)
        {
            var financial = await _service.GetByIdAsync(id);
            return financial == null ? NotFound(new { message = "Record not found" }) : Ok(financial);
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult> Create(Financial financial)
        {
            try
            {
                var created = await _service.CreateAsync(financial);
                return Ok(new
                {
                    message = "Financial record created successfully",
                    data = created
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // UPDATE (Full Replace)
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Financial financial)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, financial);
                return Ok(new
                {
                    message = "Financial record updated successfully",
                    data = updated
                });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PATCH (Partial Update - only Status)
        [HttpPatch("{id:length(24)}")]
        public async Task<IActionResult> Patch(string id, [FromBody] FinancialPatchDto dto)
        {
            try
            {
                var updated = await _service.PatchAsync(id, dto);
                return Ok(new
                {
                    message = "Status updated successfully",
                    data = updated
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { message = "Financial record deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
