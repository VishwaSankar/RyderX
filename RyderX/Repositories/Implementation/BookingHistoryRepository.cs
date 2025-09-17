using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class BookingHistoryRepository : IBookingHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookingHistory history)
        {
            try
            {
                await _context.BookingHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding booking history", ex);
            }
        }

        public async Task<IEnumerable<BookingHistory>> GetAllAsync()
        {
            try
            {
                return await _context.BookingHistories
                    .Include(b => b.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching booking histories", ex);
            }
        }

        public async Task<BookingHistory?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.BookingHistories
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching booking history {id}", ex);
            }
        }

        public async Task<IEnumerable<BookingHistory>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.BookingHistories
                    .Include(b => b.User)
                    .Where(b => b.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching booking histories for user {userId}", ex);
            }
        }

        public async Task<BookingHistory> CreateFromReservationAsync(int reservationId)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Car)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .FirstOrDefaultAsync(r => r.Id == reservationId);

                if (reservation == null)
                    throw new Exception("Invalid ReservationId");

                if (reservation.Car == null)
                    throw new Exception("Reservation has no valid car");

                var days = (reservation.DropoffAt.Date - reservation.PickupAt.Date).Days;
                if (days <= 0) throw new Exception("Invalid pickup/dropoff dates");

                var calculatedTotal = days * reservation.Car.PricePerDay;

                var history = new BookingHistory
                {
                    ReservationId = reservation.Id,
                    UserId = reservation.UserId!,
                    CarMake = reservation.Car.Make,
                    CarModel = reservation.Car.Model,
                    CarLicensePlate = reservation.Car.LicensePlate ?? "N/A",
                    PickupAt = reservation.PickupAt,
                    DropoffAt = reservation.DropoffAt,
                    PickupLocation = reservation.PickupLocation?.Name ?? "N/A",
                    DropoffLocation = reservation.DropoffLocation?.Name ?? "N/A",
                    TotalPrice = calculatedTotal,
                    Status = reservation.Status,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.BookingHistories.AddAsync(history);
                await _context.SaveChangesAsync();

                return history;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating booking history from reservation", ex);
            }
        }
    }
}
