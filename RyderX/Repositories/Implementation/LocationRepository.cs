using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            try
            {
               return await _context.Locations.ToListAsync();
            }
            catch (Exception ex)
            {

               throw new Exception("Error getting all locations", ex);
            }
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            try
            {
               return await _context.Locations.Include(l => l.Cars).FirstOrDefaultAsync(l => l.Id == id);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error getting location with id: {id}", ex);
            }
        }

        public async Task<IEnumerable<Car>> GetCarsAtLocationAsync(int locationId)
        {
            try
            {
                return await _context.Cars
                .Where(c => c.LocationId == locationId)
                .Include(c => c.Location) 
                .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching cars at location {locationId}", ex);
            }
        }

        public async Task AddAsync(Location location)
        {
            try
            {
                await _context.Locations.AddAsync(location);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Error adding location", ex);
            }
        }

        public async Task UpdateAsync(Location location)
        {
            try
            {
                _context.Locations.Update(location); 
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Error updating location", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var loc = await _context.Locations.FindAsync(id);
                if (loc != null) 
                { 
                    _context.Locations.Remove(loc); 
                    await _context.SaveChangesAsync(); 
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Error deleting location {id}", ex);
            }
        }

    }
}
