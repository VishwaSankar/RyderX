using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IExtraServiceRepository
    {
        Task<IEnumerable<ExtraService>> GetAllAsync();
        Task<ExtraService?> GetByIdAsync(int id);
        Task AddAsync(ExtraService service);
        Task UpdateAsync(ExtraService service);
        Task DeleteAsync(int id);
    }
}
