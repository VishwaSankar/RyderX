using RyderX_Server.Models;

namespace RyderX_Server.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task<IEnumerable<Car>> SearchAsync(
            string? make, string? model, string? category,
            decimal? minPrice, decimal? maxPrice,
            int? locationId, bool onlyAvailable = true);
        Task<IEnumerable<Car>> GetByLocationAsync(int locationId);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);



    }
}
