using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RyderX_Server.Authentication;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("Error updating user profile");
        }
    }
}
