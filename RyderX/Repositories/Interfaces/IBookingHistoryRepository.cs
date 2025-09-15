using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IBookingHistoryRepository
    {
        Task<IEnumerable<BookingHistory>> GetAllAsync();
        Task<BookingHistory?> GetByIdAsync(int id);
        Task<IEnumerable<BookingHistory>> GetByUserIdAsync(string userId);
        Task AddAsync(BookingHistory history);
    }
}
