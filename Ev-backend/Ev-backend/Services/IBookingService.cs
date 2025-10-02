using Ev_backend.DTOs;

namespace Ev_backend.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(CreateBookingDto dto);
        Task<BookingResponseDto> UpdateAsync(string id, UpdateBookingDto dto);
        Task<BookingResponseDto> CancelAsync(string id, CancelBookingDto dto);
        Task<BookingResponseDto> ApproveAsync(string id, ApproveBookingDto dto);
        Task<List<BookingResponseDto>> GetUpcomingAsync(string ownerNic);
        Task<List<BookingResponseDto>> GetHistoryAsync(string ownerNic);
        Task<List<BookingResponseDto>> GetAllAsync();
        Task<List<BookingResponseDto>> GetByOwnerAsync(string ownerNic);
        Task<BookingResponseDto> CompleteAsync(string id, CompletedBookingDto dto);


    }
}
