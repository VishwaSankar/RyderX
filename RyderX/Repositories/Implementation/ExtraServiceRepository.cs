using Microsoft.EntityFrameworkCore;
using RyderX_Server.Context;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Repositories.Implementation
{
    public class ExtraServiceRepository : IExtraServiceRepository
    {
        private readonly ApplicationDbContext _context;
        public ExtraServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExtraService service)
        {
            try
            {
                await _context.ExtraServices.AddAsync(service); 
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding extra service", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var service = await _context.ExtraServices.FindAsync(id);
                if (service != null) 
                { 
                    _context.ExtraServices.Remove(service); await _context.SaveChangesAsync(); 
                }
            }
            catch (Exception ex) 
            { 
                throw new Exception($"Error deleting extra service {id}", ex); 
            }
        }

        public async Task<IEnumerable<ExtraService>> GetAllAsync()
        {
            try
            {
                return await _context.ExtraServices.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Error fetching extra services", ex);
            }
        }

        public async Task<ExtraService?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.ExtraServices.FindAsync(id);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error fetching extra service {id}", ex);
            }
        }

        public async Task UpdateAsync(ExtraService service)
        {
            try 
            { 
                _context.ExtraServices.Update(service); 
                await _context.SaveChangesAsync(); 
            }
            catch (Exception ex) 
            { 
                throw new Exception("Error updating extra service", ex); 
            }
        }
    }
}
