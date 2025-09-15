using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.CarDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        // GET: api/cars
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _carRepository.GetAllAsync();
            var result = cars.Select(c => new CarDto
            {
                Id = c.Id,
                Make = c.Make,
                Model = c.Model,
                Year = c.Year,
                LicensePlate = c.LicensePlate,
                PricePerDay = c.PricePerDay,
                IsAvailable = c.IsAvailable,
                Category = c.Category,
                FuelType = c.FuelType,
                Transmission = c.Transmission,
                Seats = c.Seats,
                Features = c.Features,
                LocationName = c.Location?.Name ?? ""
            });

            return Ok(result);
        }

        // GET: api/cars/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null) return NotFound();

            var result = new CarDto
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                LicensePlate = car.LicensePlate,
                PricePerDay = car.PricePerDay,
                IsAvailable = car.IsAvailable,
                Category = car.Category,
                FuelType = car.FuelType,
                Transmission = car.Transmission,
                Seats = car.Seats,
                Features = car.Features,
                LocationName = car.Location?.Name ?? ""
            };

            return Ok(result);
        }
        // GET: api/cars/location/3
        [HttpGet("location/{locationId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarsByLocation(int locationId)
        {
            var cars = await _carRepository.GetByLocationAsync(locationId);
            if (!cars.Any())
                return NotFound(new { Message = "No cars found at this location." });

            var result = cars.Select(c => new CarDto
            {
                Id = c.Id,
                Make = c.Make,
                Model = c.Model,
                Year = c.Year,
                LicensePlate = c.LicensePlate,
                PricePerDay = c.PricePerDay,
                IsAvailable = c.IsAvailable,
                Category = c.Category,
                FuelType = c.FuelType,
                Transmission = c.Transmission,
                Seats = c.Seats,
                Features = c.Features,
                LocationName = c.Location?.Name ?? ""
            });

            return Ok(result);
        }

        // POST: api/cars
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var car = new Car
            {
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                LicensePlate = dto.LicensePlate,
                PricePerDay = dto.PricePerDay,
                IsAvailable = true,
                LocationId = dto.LocationId
            };

            await _carRepository.AddAsync(car);
            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
        }

        // PUT: api/cars
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCar([FromBody] UpdateCarDto dto)
        {
            var car = await _carRepository.GetByIdAsync(dto.Id);
            if (car == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Make)) car.Make = dto.Make;
            if (!string.IsNullOrEmpty(dto.Model)) car.Model = dto.Model;
            if (dto.Year.HasValue) car.Year = dto.Year.Value;
            if (!string.IsNullOrEmpty(dto.LicensePlate)) car.LicensePlate = dto.LicensePlate;
            if (dto.PricePerDay.HasValue) car.PricePerDay = dto.PricePerDay.Value;
            if (dto.IsAvailable.HasValue) car.IsAvailable = dto.IsAvailable.Value;
            if (dto.LocationId.HasValue) car.LocationId = dto.LocationId.Value;

            await _carRepository.UpdateAsync(car);
            return Ok(new { Message = "Car updated successfully" });
        }

        // DELETE: api/cars/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            await _carRepository.DeleteAsync(id);
            return Ok(new { Message = "Car deleted successfully" });
        }
    }
}
