using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.ExtraServiceDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraServicesController : ControllerBase
    {
        private readonly IExtraServiceRepository _extraServiceRepository;

        public ExtraServicesController(IExtraServiceRepository extraServiceRepository)
        {
            _extraServiceRepository = extraServiceRepository;
        }

        // GET: api/extraservices
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var services = await _extraServiceRepository.GetAllAsync();
                var result = services.Select(s => new ExtraServiceDto
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Price = s.Price,
                    Description = s.Description
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching extra services", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/extraservices/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var service = await _extraServiceRepository.GetByIdAsync(id);
                if (service == null) return NotFound(new { Message = "Extra service not found" });

                var dto = new ExtraServiceDto
                {
                    Id = service.Id,
                    Name = service.Name!,
                    Price = service.Price,
                    Description = service.Description
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error fetching extra service {id}", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // POST: api/extraservices
        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Create([FromBody] CreateExtraServiceDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var service = new ExtraService
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description
                };

                await _extraServiceRepository.AddAsync(service);
                return Ok(new { Message = "Extra service created successfully", ServiceId = service.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating extra service", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // PUT: api/extraservices
        [HttpPut]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> Update([FromBody] UpdateExtraServiceDto dto)
        {
            try
            {
                var service = await _extraServiceRepository.GetByIdAsync(dto.Id);
                if (service == null) return NotFound(new { Message = "Extra service not found" });

                if (!string.IsNullOrEmpty(dto.Name)) service.Name = dto.Name;
                if (dto.Price.HasValue) service.Price = dto.Price.Value;
                if (!string.IsNullOrEmpty(dto.Description)) service.Description = dto.Description;

                await _extraServiceRepository.UpdateAsync(service);
                return Ok(new { Message = "Extra service updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error updating extra service {dto.Id}", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // DELETE: api/extraservices/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _extraServiceRepository.DeleteAsync(id);
                return Ok(new { Message = "Extra service deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error deleting extra service {id}", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
