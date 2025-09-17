using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _context;

        public CarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Car car)
        {
            try
            {
                await _context.Cars.AddAsync(car);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding car", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                    throw new Exception($"No car found with id: {id}");

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting car with id: {id}", ex);
            }
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Location)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching all cars", ex);
            }
        }

        public async Task<Car?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Location)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching car {id}", ex);
            }
        }

        public async Task<IEnumerable<Car>> GetByLocationAsync(int locationId)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.Location) 
                    .Where(c => c.LocationId == locationId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching cars at location {locationId}", ex);
            }
        }

        public async Task<IEnumerable<Car>> SearchAsync(
            string? make,
            string? model,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            int? locationId,
            bool onlyAvailable = true)
        {
            try
            {
                var query = _context.Cars.AsQueryable();

                if (!string.IsNullOrEmpty(make))
                    query = query.Where(c => c.Make.Contains(make));

                if (!string.IsNullOrEmpty(model))
                    query = query.Where(c => c.Model.Contains(model));

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(c => c.Category == category);

                if (minPrice.HasValue)
                    query = query.Where(c => c.PricePerDay >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(c => c.PricePerDay <= maxPrice.Value);

                if (locationId.HasValue)
                    query = query.Where(c => c.LocationId == locationId.Value);

                if (onlyAvailable)
                    query = query.Where(c => c.IsAvailable);

                return await query
                    .Include(c => c.Location)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching cars", ex);
            }
        }

        public async Task UpdateAsync(Car car)
        {
            try
            {
                _context.Cars.Update(car);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating car", ex);
            }
        }
    }
}
