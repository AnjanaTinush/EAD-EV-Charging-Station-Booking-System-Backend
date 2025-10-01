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

        // Create booking
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id) => NotFound();

        // Update
        [HttpPut("{id}")]
        public async Task<ActionResult<BookingResponseDto>> Update(string id, [FromBody] UpdateBookingDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        // Cancel
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<BookingResponseDto>> Cancel(string id, [FromBody] CancelBookingDto dto)
        {
            var result = await _service.CancelAsync(id, dto);
            return Ok(result);
        }

        // Approve → QR
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<BookingResponseDto>> Approve(string id, [FromBody] ApproveBookingDto dto)
        {
            var result = await _service.ApproveAsync(id, dto);
            return Ok(result);
        }

        // Upcoming
        [HttpGet("upcoming")]
        public async Task<ActionResult<List<BookingResponseDto>>> Upcoming([FromQuery] string ownerNic)
        {
            var result = await _service.GetUpcomingAsync(ownerNic);
            return Ok(result);
        }

        // History
        [HttpGet("history")]
        public async Task<ActionResult<List<BookingResponseDto>>> History([FromQuery] string ownerNic)
        {
            var result = await _service.GetHistoryAsync(ownerNic);
            return Ok(result);
        }

        // GET: api/booking/all
        [HttpGet("all")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/booking/owner/{nic}
        [HttpGet("owner/{nic}")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetByOwner(string nic)
        {
            var result = await _service.GetByOwnerAsync(nic);
            return Ok(result);
        }

    }
}
