using Ev_backend.Models;

namespace Ev_backend.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking> InsertAsync(Booking booking);
        Task<Booking?> GetByIdAsync(string id);
        Task UpdateAsync(Booking booking);
        Task<List<Booking>> GetUpcomingByOwnerAsync(string ownerNic, DateTime nowUtc);
        Task<List<Booking>> GetHistoryByOwnerAsync(string ownerNic, DateTime nowUtc);
        Task<bool> ExistsOverlappingAsync(string stationId, DateTime reservationTimeUtc);
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetByOwnerAsync(string ownerNic);
    }
}
