using RyderX_Server.Authentication;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task UpdateAsync(ApplicationUser user);
    }
}
