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

        // Add a new payment
        public async Task AddAsync(Payment payment)
        {
            try
            {
                await _context.Payments.AddAsync(payment);

                var reservation = await _context.Reservations.FindAsync(payment.ReservationId);
                if (reservation != null)
                {
                    reservation.Status = "Booked";
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding payment", ex);
            }
        }

        // Get payment by Id
        public async Task<Payment?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching payment {id}", ex);
            }
        }

        // Get payment by reservation Id
        public async Task<Payment?> GetByReservationIdAsync(int reservationId)
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching payment for reservation {reservationId}", ex);
            }
        }

        // Get payments by user Id
        public async Task<IEnumerable<Payment>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                    .Where(p => p.Reservation.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching payments for user {userId}", ex);
            }
        }
        public async Task<IEnumerable<Payment>> GetByUserIdForAdminAsync(string userId)
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Reservation)
                    .ThenInclude(r => r.User)
                    .Where(p => p.Reservation.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching payments for user {userId} (Admin)", ex);
            }
        }

    }
}
