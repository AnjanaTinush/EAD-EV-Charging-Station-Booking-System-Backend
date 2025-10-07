using Ev_backend.DTOs;
using Ev_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ev_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service) => _service = service;

        // CREATE BOOKING
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id) => NotFound();

        // UPDATE BOOKING
        [HttpPut("{id}")]
        public async Task<ActionResult<BookingResponseDto>> Update(string id, [FromBody] UpdateBookingDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        // CANCEL BOOKING
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<BookingResponseDto>> Cancel(string id, [FromBody] CancelBookingDto dto)
        {
            var result = await _service.CancelAsync(id, dto);
            return Ok(result);
        }

        // APPROVE BOOKING
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<BookingResponseDto>> Approve(string id)
        {
            var result = await _service.ApproveAsync(id);
            return Ok(result);
        }


        // COMPLETE BOOKING
        [HttpPost("{id}/complete")]
        public async Task<ActionResult<BookingResponseDto>> Complete(string id, [FromBody] CompletedBookingDto dto)
        {
            var result = await _service.CompleteAsync(id, dto);
            return Ok(result);
        }

        // UPCOMING BOOKINGS (by NIC)
        [HttpGet("upcoming")]
        public async Task<ActionResult<List<BookingResponseDto>>> Upcoming([FromQuery] string ownerNic)
        {
            var result = await _service.GetUpcomingAsync(ownerNic);
            return Ok(result);
        }

        // HISTORY BOOKINGS (by NIC)
        [HttpGet("history")]
        public async Task<ActionResult<List<BookingResponseDto>>> History([FromQuery] string ownerNic)
        {
            var result = await _service.GetHistoryAsync(ownerNic);
            return Ok(result);
        }

        // ALL BOOKINGS
        [HttpGet("all")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // BOOKINGS BY OWNER (NIC)
        [HttpGet("owner/{nic}")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetByOwner(string nic)
        {
            var result = await _service.GetByOwnerAsync(nic);
            return Ok(result);
        }

        // DELETE BOOKING
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
