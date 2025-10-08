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
        private readonly UserRepository _userRepo;
        private readonly ITimeProvider _clock;

        public BookingService(IBookingRepository repo, UserRepository userRepo, ITimeProvider clock)
        {
            _repo = repo;
            _userRepo = userRepo;
            _clock = clock;
        }

        // ====================== CREATE ======================
        // ====================== CREATE ======================
        public async Task<BookingResponseDto> CreateAsync(CreateBookingDto dto)
        {
            var now = _clock.UtcNow;

            // Validate EV Owner
            var user = await _userRepo.GetByNICAsync(dto.OwnerNIC);
            if (user == null || !user.IsActive || user.Role != UserRole.EvOwner)
                throw new UnauthorizedAccessException("Booking not allowed. User not found or not an active EvOwner.");

            // Booking date rule
            if (dto.ReservationTime < now || dto.ReservationTime > now.AddDays(7))
                throw new InvalidOperationException("Reservation time must be within 7 days from now.");

            // Check for existing booking conflicts (ignore Cancelled or Completed)
            var existing = await _repo.GetByStationAndTimeAsync(dto.StationId, dto.ReservationTime);
            if (existing.Any(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Completed))
                throw new InvalidOperationException("Another active booking already exists at this time.");

            // Create new booking
            var booking = new Booking
            {
                OwnerNIC = dto.OwnerNIC,
                StationId = dto.StationId,
                ReservationTime = dto.ReservationTime,
                Status = BookingStatus.Pending,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _repo.InsertAsync(booking);
            return Map(booking);
        }

        // ====================== UPDATE ======================
        public async Task<BookingResponseDto> UpdateAsync(string id, UpdateBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            var now = _clock.UtcNow;

            EnsurePending(booking);
            EnsureBefore12h(booking.ReservationTime, now);

            if (dto.NewReservationTime < now || dto.NewReservationTime > now.AddDays(7))
                throw new InvalidOperationException("New reservation time must be within 7 days.");

            // Check for overlapping bookings except Cancelled or Completed
            var existing = await _repo.GetByStationAndTimeAsync(booking.StationId, dto.NewReservationTime);
            if (existing.Any(b =>
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Completed &&
                b.Id != booking.Id))
                throw new InvalidOperationException("Another active booking already exists at this time.");

            booking.ReservationTime = dto.NewReservationTime;
            booking.UpdatedAt = now;

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }


        // ====================== CANCEL ======================
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

        // ====================== APPROVE ======================
        public async Task<BookingResponseDto> ApproveAsync(string id)
        {
            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            EnsurePending(booking);

            booking.Status = BookingStatus.Approved;
            booking.ApprovedAt = _clock.UtcNow;
            booking.UpdatedAt = booking.ApprovedAt.Value;

            var payload = $"booking:{booking.Id}|owner:{booking.OwnerNIC}|station:{booking.StationId}|time:{booking.ReservationTime:o}";
            booking.QrCodeBase64 = GenerateQrBase64(payload);

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }

        // ====================== COMPLETE ======================
        public async Task<BookingResponseDto> CompleteAsync(string id, CompletedBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Booking not found");
            if (booking.Status != BookingStatus.Approved)
                throw new InvalidOperationException("Only approved bookings can be completed.");

            booking.Status = BookingStatus.Completed;
            booking.CompletedAt = _clock.UtcNow;
            booking.UpdatedAt = booking.CompletedAt.Value;

            await _repo.UpdateAsync(booking);
            return Map(booking);
        }

        // ====================== GET METHODS ======================
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

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");

            await _repo.DeleteByIdAsync(id);
            return true;
        }

        // ====================== HELPERS ======================
        private static void EnsurePending(Booking b)
        {
            if (b.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Only pending bookings can be modified.");
        }

        private static void EnsureBefore12h(DateTime resUtc, DateTime nowUtc)
        {
            if (resUtc - nowUtc < TimeSpan.FromHours(12))
                throw new InvalidOperationException("Operation not allowed within 12 hours of reservation.");
        }

        private static string GenerateQrBase64(string payload)
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qr = new PngByteQRCode(data);
            var bytes = qr.GetGraphic(20);
            return Convert.ToBase64String(bytes);
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
    }
}
