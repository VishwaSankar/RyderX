using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.LocationDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        public LocationsController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        // GET: api/locations
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllLocations()
        {
            try
            {
                var locations = await _locationRepository.GetAllAsync();

                var result = locations.Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    City = l.City,
                    State = l.State,
                    ZipCode = l.ZipCode,
                    Country = l.Country
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching locations", Details = ex.Message });
            }
        }

        // GET: api/locations/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLocationById(int id)
        {
            try
            {
                var location = await _locationRepository.GetByIdAsync(id);
                if (location == null) return NotFound(new { Message = "Location not found" });

                var dto = new LocationDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    City = location.City,
                    State = location.State,
                    ZipCode = location.ZipCode,
                    Country = location.Country
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching location", Details = ex.Message });
            }
        }

        // POST: api/locations
        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> CreateLocation([FromBody] CreateLocationDto dto)
        {
            try
            {
                var location = new Location
                {
                    Name = dto.Name,
                    City = dto.City,
                    State = dto.State,
                    ZipCode = dto.ZipCode,
                    Country = dto.Country
                };

                await _locationRepository.AddAsync(location);
                return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating location", Details = ex.Message });
            }
        }

        // PUT: api/locations
        [HttpPut]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        {
            try
            {
                var location = await _locationRepository.GetByIdAsync(dto.Id);
                if (location == null) return NotFound(new { Message = "Location not found" });

                if (!string.IsNullOrEmpty(dto.Name)) location.Name = dto.Name;
                if (!string.IsNullOrEmpty(dto.City)) location.City = dto.City;
                if (!string.IsNullOrEmpty(dto.State)) location.State = dto.State;
                if (!string.IsNullOrEmpty(dto.ZipCode)) location.ZipCode = dto.ZipCode;
                if (!string.IsNullOrEmpty(dto.Country)) location.Country = dto.Country;

                await _locationRepository.UpdateAsync(location);
                return Ok(new { Message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating location", Details = ex.Message });
            }
        }

        // DELETE: api/locations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                var location = await _locationRepository.GetByIdAsync(id);
                if (location == null) return NotFound(new { Message = "Location not found" });

                // check if cars exist before deleting
                if (location.Cars != null && location.Cars.Any())
                {
                    return BadRequest(new { Message = "Cannot delete location with cars assigned. Remove or transfer cars first." });
                }

                await _locationRepository.DeleteAsync(id);
                return Ok(new { Message = "Location deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting location", Details = ex.Message });
            }
        }

        // GET: api/locations/{id}/cars
        [HttpGet("{id}/cars")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarsAtLocation(int id)
        {
            try
            {
                var cars = await _locationRepository.GetCarsAtLocationAsync(id);
                if (!cars.Any()) return NotFound(new { Message = "No cars found at this location" });

                var result = cars.Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.Year,
                    c.PricePerDay,
                    c.IsAvailable,
                    c.LicensePlate,
                    LocationName = c.Location?.Name ?? "Unknown"
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching cars for location", Details = ex.Message });
            }
        }
    }
}
