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
            try
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
                    OwnerId = c.OwnerId,
                    OwnerName = c.Owner != null ? $"{c.Owner.FirstName} {c.Owner.LastName}" : "Unknown",
                    LocationName = c.Location?.Name ?? "",
                    ImageUrl = c.ImageUrl
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching cars", Details = ex.Message });
            }
        }

        // GET: api/cars/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarById(int id)
        {
            try
            {
                var car = await _carRepository.GetByIdAsync(id);
                if (car == null) return NotFound(new { Message = "Car not found" });

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
                    OwnerId = car.OwnerId,
                    OwnerName = car.Owner != null ? $"{car.Owner.FirstName} {car.Owner.LastName}" : "Unknown",
                    LocationName = car.Location?.Name ?? "",
                    ImageUrl = car.ImageUrl
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error fetching car {id}", Details = ex.Message });
            }
        }

        // GET: api/cars/location/3
        [HttpGet("location/{locationId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarsByLocation(int locationId)
        {
            try
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
                    OwnerId = c.OwnerId,
                    OwnerName = c.Owner != null ? $"{c.Owner.FirstName} {c.Owner.LastName}" : "Unknown",
                    LocationName = c.Location?.Name ?? "",
                    ImageUrl = c.ImageUrl
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error fetching cars for location {locationId}", Details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> CreateCar([FromForm] CreateCarDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Invalid user" });

                string? savedImagePath = null;
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cars");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(stream);
                    }

                    savedImagePath = $"/uploads/cars/{fileName}";
                }

                var car = new Car
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    Year = dto.Year,
                    LicensePlate = dto.LicensePlate,
                    PricePerDay = dto.PricePerDay,
                    IsAvailable = true,
                    LocationId = dto.LocationId,
                    Category = dto.Category,
                    FuelType = dto.FuelType,
                    Transmission = dto.Transmission,
                    Seats = dto.Seats,
                    Features = dto.Features,
                    OwnerId = userId,
                    ImageUrl = savedImagePath
                };

                await _carRepository.AddAsync(car);

                return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, new CarDto
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
                    OwnerId = car.OwnerId,
                    ImageUrl = car.ImageUrl,
                    LocationName = car.Location?.Name ?? ""
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating car", Details = ex.Message });
            }
        }

        [HttpGet("mycars")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> GetMyCars()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Invalid user" });

            var cars = await _carRepository.GetAllAsync();
            var myCars = cars.Where(c => c.OwnerId == userId);

            var result = myCars.Select(c => new CarDto
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
                OwnerId = c.OwnerId,
                OwnerName = c.Owner != null ? $"{c.Owner.FirstName} {c.Owner.LastName}" : "Unknown",
                LocationName = c.Location?.Name ?? "",
                ImageUrl = c.ImageUrl
            });

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateCar(int id, [FromForm] UpdateCarDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { Message = "Car ID mismatch" });

                var car = await _carRepository.GetByIdAsync(dto.Id);
                if (car == null) return NotFound(new { Message = "Car not found" });

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (User.IsInRole("Agent") && car.OwnerId != userId)
                    return Forbid();

                // ✅ update fields only if provided
                if (!string.IsNullOrEmpty(dto.Make)) car.Make = dto.Make;
                if (!string.IsNullOrEmpty(dto.Model)) car.Model = dto.Model;
                if (dto.Year.HasValue) car.Year = dto.Year.Value;
                if (!string.IsNullOrEmpty(dto.LicensePlate)) car.LicensePlate = dto.LicensePlate;
                if (dto.PricePerDay.HasValue) car.PricePerDay = dto.PricePerDay.Value;
                if (dto.IsAvailable.HasValue) car.IsAvailable = dto.IsAvailable.Value;
                if (dto.LocationId.HasValue) car.LocationId = dto.LocationId.Value;
                if (!string.IsNullOrEmpty(dto.Category)) car.Category = dto.Category;
                if (!string.IsNullOrEmpty(dto.FuelType)) car.FuelType = dto.FuelType;
                if (!string.IsNullOrEmpty(dto.Transmission)) car.Transmission = dto.Transmission;
                if (dto.Seats.HasValue) car.Seats = dto.Seats.Value;
                if (!string.IsNullOrEmpty(dto.Features)) car.Features = dto.Features;

                // ✅ handle image update if new file is uploaded
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cars");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(stream);
                    }

                    car.ImageUrl = $"/uploads/cars/{fileName}";
                }

                car.UpdatedDate = DateTime.UtcNow;

                await _carRepository.UpdateAsync(car);

                return Ok(new { Message = "Car updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating car", Details = ex.Message });
            }
        }



        [HttpPut("my/{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateMyCar(int id, [FromForm] UpdateCarDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { Message = "Car ID mismatch" });

                var car = await _carRepository.GetByIdAsync(dto.Id);
                if (car == null) return NotFound(new { Message = "Car not found" });

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Invalid user" });

                // ✅ Only allow Agents to update their own cars
                if (User.IsInRole("Agent") && car.OwnerId != userId)
                    return Forbid();

                // ✅ Admin can update any car
                if (!string.IsNullOrEmpty(dto.Make)) car.Make = dto.Make;
                if (!string.IsNullOrEmpty(dto.Model)) car.Model = dto.Model;
                if (dto.Year.HasValue) car.Year = dto.Year.Value;
                if (!string.IsNullOrEmpty(dto.LicensePlate)) car.LicensePlate = dto.LicensePlate;
                if (dto.PricePerDay.HasValue) car.PricePerDay = dto.PricePerDay.Value;
                if (dto.IsAvailable.HasValue) car.IsAvailable = dto.IsAvailable.Value;
                if (dto.LocationId.HasValue) car.LocationId = dto.LocationId.Value;
                if (!string.IsNullOrEmpty(dto.Category)) car.Category = dto.Category;
                if (!string.IsNullOrEmpty(dto.FuelType)) car.FuelType = dto.FuelType;
                if (!string.IsNullOrEmpty(dto.Transmission)) car.Transmission = dto.Transmission;
                if (dto.Seats.HasValue) car.Seats = dto.Seats.Value;
                if (!string.IsNullOrEmpty(dto.Features)) car.Features = dto.Features;

                // ✅ handle image update if new file is uploaded
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cars");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(stream);
                    }

                    car.ImageUrl = $"/uploads/cars/{fileName}";
                }

                car.UpdatedDate = DateTime.UtcNow;

                await _carRepository.UpdateAsync(car);

                return Ok(new { Message = "Car updated successfully", CarId = car.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating car", Details = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _carRepository.GetByIdAsync(id);
                if (car == null) return NotFound(new { Message = "Car not found" });

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (User.IsInRole("Agent") && car.OwnerId != userId)
                    return Forbid();

                await _carRepository.DeleteAsync(id);
                return Ok(new { Message = "Car deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error deleting car {id}", Details = ex.Message });
            }
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableCars()
        {
            try
            {
                var cars = await _carRepository.GetAllAsync();
                var availableCars = cars.Where(c => c.IsAvailable);

                var result = availableCars.Select(c => new CarDto
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
                    OwnerId = c.OwnerId,
                    OwnerName = c.Owner != null ? $"{c.Owner.FirstName} {c.Owner.LastName}" : "Unknown",
                    LocationName = c.Location?.Name ?? "",
                    ImageUrl = c.ImageUrl
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching available cars", Details = ex.Message });
            }
        }

        [HttpGet("available/location/{locationName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableCarsByLocation(string locationName)
        {
            try
            {
                var cars = await _carRepository.GetAllAsync();

                var availableCars = cars
                    .Where(c => c.IsAvailable && c.Location != null &&
                                c.Location.Name.ToLower() == locationName.ToLower());

                if (!availableCars.Any())
                    return NotFound(new { Message = $"No available cars found at location {locationName}" });

                var result = availableCars.Select(c => new CarDto
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
                    OwnerId = c.OwnerId,
                    OwnerName = c.Owner != null ? $"{c.Owner.FirstName} {c.Owner.LastName}" : "Unknown",
                    LocationName = c.Location?.Name ?? "",
                    ImageUrl = c.ImageUrl
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error fetching cars for location {locationName}", Details = ex.Message });
            }
        }
    }
}
