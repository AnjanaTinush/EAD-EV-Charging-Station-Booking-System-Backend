using Ev_backend.Models;
using MongoDB.Driver;

namespace Ev_backend.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IMongoCollection<Booking> _collection;

        public BookingRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<Booking>("bookings");
        }

        public async Task InsertAsync(Booking booking)
        {
            await _collection.InsertOneAsync(booking);
        }

        public async Task<Booking?> GetByIdAsync(string id)
        {
            return await _collection.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            await _collection.ReplaceOneAsync(b => b.Id == booking.Id, booking);
        }

        public async Task DeleteByIdAsync(string id)
        {
            await _collection.DeleteOneAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetByOwnerAsync(string ownerNic)
        {
            return await _collection.Find(b => b.OwnerNIC == ownerNic).ToListAsync();
        }

        public async Task<List<Booking>> GetUpcomingByOwnerAsync(string ownerNic, DateTime nowUtc)
        {
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.OwnerNIC, ownerNic),
                Builders<Booking>.Filter.Gt(b => b.ReservationTime, nowUtc)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<Booking>> GetHistoryByOwnerAsync(string ownerNic, DateTime nowUtc)
        {
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.OwnerNIC, ownerNic),
                Builders<Booking>.Filter.Lt(b => b.ReservationTime, nowUtc)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        // ✅ Used for booking conflict check
        public async Task<List<Booking>> GetByStationAndTimeAsync(string stationId, DateTime time)
        {
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(b => b.StationId, stationId),
                Builders<Booking>.Filter.Eq(b => b.ReservationTime, time)
            );
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
