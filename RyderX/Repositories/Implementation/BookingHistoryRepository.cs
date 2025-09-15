using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class BookingHistoryRepository : IBookingHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BookingHistory history)
        {
            try 
            { 
                await _context.BookingHistories.AddAsync(history); 
                await _context.SaveChangesAsync(); 
            }
            catch (Exception ex) 
            { 
                throw new Exception("Error adding booking history", ex); 
            }
        }

        public async Task<IEnumerable<BookingHistory>> GetAllAsync()
        {
            try
            {
                return await _context.BookingHistories.Include(b => b.User).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Error fetching booking histories", ex);
            }
        }

        public async Task<BookingHistory?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.BookingHistories.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching booking history {id}", ex);
            }
        }

        public async Task<IEnumerable<BookingHistory>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.BookingHistories.Where(b => b.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching booking histories for user {userId}", ex);
            }
        }
    }
}
