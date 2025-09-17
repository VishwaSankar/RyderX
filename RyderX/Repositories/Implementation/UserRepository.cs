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
            try
            {
                return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user with Id {userId}: {ex.Message}", ex);
            }
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Error updating user profile: {errors}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while updating user {user.Id}: {ex.Message}", ex);
            }
        }
    }
}
