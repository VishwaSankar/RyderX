using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByReservationIdAsync(int reservationId);
        Task<IEnumerable<Payment>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Payment>> GetByUserIdForAdminAsync(string userId); 
        Task AddAsync(Payment payment);
        Task<IEnumerable<Payment>> GetByOwnerIdAsync(string ownerId); 

    }
}
