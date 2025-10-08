using Ev_backend.Models;

namespace Ev_backend.Repositories
{
    public interface IBookingRepository
    {
        Task InsertAsync(Booking booking);
        Task<Booking?> GetByIdAsync(string id);
        Task<List<Booking>> GetAllAsync();
        Task UpdateAsync(Booking booking);
        Task DeleteByIdAsync(string id);
        Task<List<Booking>> GetByOwnerAsync(string ownerNic);
        Task<List<Booking>> GetUpcomingByOwnerAsync(string ownerNic, DateTime nowUtc);
        Task<List<Booking>> GetHistoryByOwnerAsync(string ownerNic, DateTime nowUtc);

        // ✅ For create booking overlap check (ignore cancelled ones)
        Task<List<Booking>> GetByStationAndTimeAsync(string stationId, DateTime time);
    }
}
