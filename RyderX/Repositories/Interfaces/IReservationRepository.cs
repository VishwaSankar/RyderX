using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetByCarIdAsync(int carId);
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync(string userId);
        Task AddAsync(Reservation reservation);
        Task UpdateStatusAsync(int id, string status); // Booked, Cancelled, Completed
        Task CancelAsync(int id);
    }
}
