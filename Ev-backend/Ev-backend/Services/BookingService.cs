using Ev_backend.DTOs;
using Ev_backend.Models;
using Ev_backend.Models.Enums;
using Ev_backend.Repositories;
using Ev_backend.Utils;
using QRCoder;

namespace Ev_backend.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IEVOwnerRepository _ownerRepo;   // check NIC exists
        private readonly ITimeProvider _clock;

        public BookingService(IBookingRepository repo, IEVOwnerRepository ownerRepo, ITimeProvider clock)
        {
            _repo = repo;
            _ownerRepo = ownerRepo;
            _clock = clock;
        }

        public async Task<BookingResponseDto> CreateAsync(CreateBookingDto dto)
        {
            var now = _clock.UtcNow;
            var resTime = dto.ReservationTime;

            // ✅ Check NIC exists in EV Owner DB
            var owner = await _ownerRepo.GetByNICAsync(dto.OwnerNIC);
            if (owner == null || !owner.IsActive)
                throw new UnauthorizedAccessException("Booking not allowed. EV Owner with this NIC does not exist or is inactive.");

            // Rule: within 7 days
            if (resTime < now || resTime > now.AddDays(7))
                throw new InvalidOperationException("Reservation time must be within 7 days from now and in the future.");

            // Prevent overlapping
            if (await _repo.ExistsOverlappingAsync(dto.StationId, resTime))
                throw new InvalidOperationException("Another booking exists for this station at this time.");

            var entity = new Booking
            {
                OwnerNIC = dto.OwnerNIC,
                StationId = dto.StationId,
                ReservationTime = resTime,
                Status = BookingStatus.Pending,
                CreatedAt = now,
                UpdatedAt = now
            };

            entity = await _repo.InsertAsync(entity);
            return Map(entity);
        }

        public async Task<BookingResponseDto> UpdateAsync(string id, UpdateBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            var now = _clock.UtcNow;

            EnsurePending(booking);
            EnsureBefore12h(booking.ReservationTime, now);

            if (dto.NewReservationTime < now || dto.NewReservationTime > now.AddDays(7))
                throw new InvalidOperationException("New reservation time must be within 7 days from now and in the future.");

            if (await _repo.ExistsOverlappingAsync(booking.StationId, dto.NewReservationTime))
                throw new InvalidOperationException("Another booking exists for this station at this time.");

            booking.ReservationTime = dto.NewReservationTime;
            booking.UpdatedAt = now;

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }

        public async Task<BookingResponseDto> CancelAsync(string id, CancelBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            var now = _clock.UtcNow;

            EnsurePending(booking);
            EnsureBefore12h(booking.ReservationTime, now);

            booking.Status = BookingStatus.Cancelled;
            booking.CancelReason = dto.Reason;
            booking.CancelledAt = now;
            booking.UpdatedAt = now;

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }

        public async Task<BookingResponseDto> ApproveAsync(string id, ApproveBookingDto dto)
        {
            if (!dto.Approve) throw new InvalidOperationException("Only approval is supported.");

            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            EnsurePending(booking);

            booking.Status = BookingStatus.Approved;
            booking.ApprovedAt = _clock.UtcNow;
            booking.UpdatedAt = booking.ApprovedAt.Value;

            // Generate QR
            var payload = $"booking:{booking.Id}|owner:{booking.OwnerNIC}|station:{booking.StationId}|time:{booking.ReservationTime:o}";
            booking.QrCodeBase64 = GenerateQrBase64(payload);

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }

        public async Task<List<BookingResponseDto>> GetUpcomingAsync(string ownerNic)
        {
            var list = await _repo.GetUpcomingByOwnerAsync(ownerNic, _clock.UtcNow);
            return list.Select(Map).ToList();
        }

        public async Task<List<BookingResponseDto>> GetHistoryAsync(string ownerNic)
        {
            var list = await _repo.GetHistoryByOwnerAsync(ownerNic, _clock.UtcNow);
            return list.Select(Map).ToList();
        }

        // Helpers
        private static void EnsureBefore12h(DateTime resUtc, DateTime nowUtc)
        {
            if (resUtc - nowUtc < TimeSpan.FromHours(12))
                throw new InvalidOperationException("Operation not allowed within 12 hours of reservation time.");
        }

        private static void EnsurePending(Booking b)
        {
            if (b.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Only pending bookings can be modified/approved/cancelled.");
        }

        private static BookingResponseDto Map(Booking b) => new()
        {
            Id = b.Id,
            OwnerNIC = b.OwnerNIC,
            StationId = b.StationId,
            ReservationTime = b.ReservationTime,
            Status = b.Status,
            QrCodeBase64 = b.QrCodeBase64,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        };

        private static string GenerateQrBase64(string payload)
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qr = new PngByteQRCode(data);
            var bytes = qr.GetGraphic(20);
            return Convert.ToBase64String(bytes);
        }

        public async Task<List<BookingResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(Map).ToList();
        }

        public async Task<List<BookingResponseDto>> GetByOwnerAsync(string ownerNic)
        {
            var list = await _repo.GetByOwnerAsync(ownerNic);
            return list.Select(Map).ToList();
        }

        public async Task<BookingResponseDto> CompleteAsync(string id, CompletedBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            var now = _clock.UtcNow;

            // Only Approved bookings can be marked as Completed
            if (booking.Status != BookingStatus.Approved)
                throw new InvalidOperationException("Only approved bookings can be completed.");

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = now;

            // Optional: you could store notes in a new field if you want
            // booking.CompletionNotes = dto.Notes;

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            await _repo.DeleteByIdAsync(id);
            return true;
        }



    }
}
