using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<IEnumerable<Announcement>> GetAllAsync();          // for Admin/Agent
        Task<IEnumerable<Announcement>> GetActiveAsync();       // for Users
        Task<Announcement?> GetByIdAsync(int id);
        Task AddAsync(Announcement announcement);
        Task UpdateAsync(Announcement announcement);
        Task DeleteAsync(int id);
    }
}
