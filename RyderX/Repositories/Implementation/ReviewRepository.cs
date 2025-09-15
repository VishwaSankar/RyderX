using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class ReviewRepository :IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            try 
            { 
                await _context.Reviews.AddAsync(review); 
                await _context.SaveChangesAsync(); 
            }
            catch (Exception ex) 
            { 
                throw new Exception("Error adding review", ex); 
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review != null) 
                { 
                    _context.Reviews.Remove(review); 
                    await _context.SaveChangesAsync(); 
                }
            }
            catch (Exception ex) 
            { 
                throw new Exception($"Error deleting review {id}", ex); 
            }
        }

        public async Task<double> GetAverageRatingAsync(int carId)
        {
            try
            {
                return await _context.Reviews.Where(r => r.CarId == carId).AverageAsync(r => (double?)r.Rating) ?? 0;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error calculating average rating for car {carId}", ex);
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId)
        {
            try
            {
                return await _context.Reviews.Include(u => u.User).Where(r => r.CarId == carId).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching reviews for car {carId}", ex);
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId)
        {
            try
            {
               return await _context.Reviews.Include(r => r.Car).Where(r => r.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching reviews by user {userId}", ex);
            }
        }
    }
}
