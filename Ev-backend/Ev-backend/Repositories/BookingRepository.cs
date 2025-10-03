using Ev_backend.Config;
using Ev_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IMongoCollection<Booking> _col;

        public BookingRepository(IOptions<MongoDbSettings> opt)
        {
            var client = new MongoClient(opt.Value.ConnectionString);
            var db = client.GetDatabase(opt.Value.DatabaseName);
            _col = db.GetCollection<Booking>("bookings");
        }

        public async Task<Booking> InsertAsync(Booking booking)
        {
            await _col.InsertOneAsync(booking);
            return booking;
        }

        public Task<Booking?> GetByIdAsync(string id) =>
            _col.Find(b => b.Id == id).FirstOrDefaultAsync();

        public Task UpdateAsync(Booking booking) =>
            _col.ReplaceOneAsync(b => b.Id == booking.Id, booking);

        public Task<List<Booking>> GetUpcomingByOwnerAsync(string ownerNic, DateTime nowUtc) =>
            _col.Find(b => b.OwnerNIC == ownerNic && b.ReservationTime >= nowUtc)
                .SortBy(b => b.ReservationTime)
                .ToListAsync();

        public Task<List<Booking>> GetHistoryByOwnerAsync(string ownerNic, DateTime nowUtc) =>
            _col.Find(b => b.OwnerNIC == ownerNic && b.ReservationTime < nowUtc)
                .SortByDescending(b => b.ReservationTime)
                .ToListAsync();

        public Task<bool> ExistsOverlappingAsync(string stationId, DateTime reservationTimeUtc) =>
            _col.Find(b => b.StationId == stationId && b.ReservationTime == reservationTimeUtc)
                .AnyAsync();

        public Task<List<Booking>> GetAllAsync() =>
            _col.Find(_ => true).SortByDescending(b => b.CreatedAt).ToListAsync();

        public Task<List<Booking>> GetByOwnerAsync(string ownerNic) =>
            _col.Find(b => b.OwnerNIC == ownerNic)
                .SortByDescending(b => b.ReservationTime)
                .ToListAsync();

        public async Task DeleteByIdAsync(string id)
        {
            await _col.DeleteOneAsync(b => b.Id == id);
        }

    }
}
