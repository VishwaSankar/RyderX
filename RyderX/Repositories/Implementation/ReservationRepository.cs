using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;
        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Reservation reservation)
        {
            try
            {
               await _context.Reservations.AddAsync(reservation);
               await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Error adding reservation", ex);
            }
        }

        public async Task CancelAsync(int id)
        {
            try
            {
                var res = await _context.Reservations.FindAsync(id);
                if (res != null)
                {
                    res.Status = "Cancelled";
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error cancelling reservation {id}", ex);
            }
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync(string userId)
        {
            try
            {
                 return await _context.Reservations
                .Where(r => r.UserId == userId && r.Status == "Booked")
                .Include(r => r.Car)
                .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching active reservations for user {userId}", ex);
            }
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            try
            {
                 return await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.PickupLocation)
                .Include(r => r.DropoffLocation)
                .Include(r => r.User)
                .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Error fetching reservations", ex);
            }
        }

        public async Task<IEnumerable<Reservation>> GetByCarIdAsync(int carId)
        {
            try
            {
                return await _context.Reservations.Where(r => r.CarId == carId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching reservations for car {carId}", ex);
            }
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Reservations
               .Include(r => r.Car)
               .Include(r => r.User)
               .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex) 
            { 
                throw new Exception($"Error fetching reservation {id}", ex); 
            }
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Reservations
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Car)
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching reservations for user {userId}", ex);
            }
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            try
            {
                var res = await _context.Reservations.FindAsync(id);
                if (res != null)
                {
                    res.Status = status;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception($"Error updating reservation {id}", ex); }
        }
    }
}
