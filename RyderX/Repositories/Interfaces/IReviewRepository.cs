using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsByCarIdAsync(int carId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId);
        Task<double> GetAverageRatingAsync(int carId);
        Task AddAsync(Review review);
        Task DeleteAsync(int id);
    }
}
