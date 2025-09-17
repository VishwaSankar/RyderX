using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            try
            {
                return await _context.Announcements
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching all announcements", ex);
            }
        }

        public async Task<IEnumerable<Announcement>> GetActiveAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _context.Announcements
                    .Where(a => a.IsActive && (a.ExpiryDate == null || a.ExpiryDate > now))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching active announcements", ex);
            }
        }

        public async Task<Announcement?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Announcements.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching announcement {id}", ex);
            }
        }

        public async Task AddAsync(Announcement announcement)
        {
            try
            {
                await _context.Announcements.AddAsync(announcement);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding announcement", ex);
            }
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            try
            {
                _context.Announcements.Update(announcement);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating announcement {announcement.Id}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var announcement = await _context.Announcements.FindAsync(id);
                if (announcement != null)
                {
                    _context.Announcements.Remove(announcement);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting announcement {id}", ex);
            }
        }
    }
}
