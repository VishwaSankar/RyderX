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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var car = await _context.Cars.FindAsync(reservation.CarId);
                if (car == null) throw new Exception("Car not found");

                if (!car.IsAvailable) throw new Exception("Car is already reserved/unavailable");

                // mark car unavailable when reservation is created
                car.IsAvailable = false;
                _context.Cars.Update(car);

                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw new Exception($"Error adding reservation: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task CancelAsync(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Car)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation != null)
                {
                    reservation.Status = "Cancelled";

                    if (reservation.Car != null)
                    {
                        // free car on cancel
                        reservation.Car.IsAvailable = true;
                        _context.Cars.Update(reservation.Car);
                    }

                    await _context.SaveChangesAsync();

                    // log booking history
                    var history = new BookingHistory
                    {
                        ReservationId = reservation.Id,
                        UserId = reservation.UserId!,
                        CarMake = reservation.Car?.Make ?? "Unknown",
                        CarModel = reservation.Car?.Model ?? "Unknown",
                        CarLicensePlate = reservation.Car?.LicensePlate ?? "N/A",
                        PickupAt = reservation.PickupAt,
                        DropoffAt = reservation.DropoffAt,
                        PickupLocation = reservation.PickupLocation?.Name ?? "N/A",
                        DropoffLocation = reservation.DropoffLocation?.Name ?? "N/A",
                        TotalPrice = reservation.TotalPrice,
                        Status = "Cancelled",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.BookingHistories.Add(history);
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
                return await _context.Reservations
                    .Where(r => r.CarId == carId)
                    .Include(r => r.Car)
                    .ToListAsync();
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
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
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
                    .Include(r => r.User)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
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
                var reservation = await _context.Reservations
                    .Include(r => r.Car)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation != null)
                {
                    reservation.Status = status;

                    if (status == "Completed" || status == "Cancelled")
                    {
                        if (reservation.Car != null)
                        {
                            // free car on completion/cancellation
                            reservation.Car.IsAvailable = true;
                            _context.Cars.Update(reservation.Car);
                        }

                        var history = new BookingHistory
                        {
                            ReservationId = reservation.Id,
                            UserId = reservation.UserId!,
                            CarMake = reservation.Car?.Make ?? "Unknown",
                            CarModel = reservation.Car?.Model ?? "Unknown",
                            CarLicensePlate = reservation.Car?.LicensePlate ?? "N/A",
                            PickupAt = reservation.PickupAt,
                            DropoffAt = reservation.DropoffAt,
                            PickupLocation = reservation.PickupLocation?.Name ?? "N/A",
                            DropoffLocation = reservation.DropoffLocation?.Name ?? "N/A",
                            TotalPrice = reservation.TotalPrice,
                            Status = reservation.Status,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.BookingHistories.Add(history);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating reservation {id}", ex);
            }
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdForAdminAsync(string userId)
        {
            try
            {
                return await _context.Reservations
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Car)
                    .Include(r => r.PickupLocation)
                    .Include(r => r.DropoffLocation)
                    .Include(r => r.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching reservations for user {userId} (Admin)", ex);
            }
        }
    }
}
