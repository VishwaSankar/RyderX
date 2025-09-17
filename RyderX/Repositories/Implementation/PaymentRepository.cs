using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            try
            {
                // ✅ Get reservation with car
                var reservation = await _context.Reservations
                    .Include(r => r.Car)
                    .FirstOrDefaultAsync(r => r.Id == payment.ReservationId);

                if (reservation == null) throw new Exception("Reservation not found");
                if (reservation.Car == null) throw new Exception("Car not found for reservation");

                var days = (reservation.DropoffAt.Date - reservation.PickupAt.Date).Days;
                if (days <= 0) throw new Exception("Invalid reservation dates");

                // ✅ Calculate correct amount
                var calculatedAmount = days * reservation.Car.PricePerDay;

                // ✅ Overwrite amount (ignore client value)
                payment.Amount = calculatedAmount;

                await _context.Payments.AddAsync(payment);

                // Mark reservation as booked after payment
                reservation.Status = "Booked";
                reservation.TotalPrice = calculatedAmount;
                _context.Reservations.Update(reservation);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding payment", ex);
            }
        }

        // Other methods unchanged...
        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.User)
                .Where(p => p.Reservation.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserIdForAdminAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.User)
                .Where(p => p.Reservation.UserId == userId)
                .ToListAsync();
        }
    }
}
