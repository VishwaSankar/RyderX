using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.ReservationDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICarRepository _carRepository;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(
            IReservationRepository reservationRepository,
            ICarRepository carRepository,
            ILogger<ReservationsController> logger)
        {
            _reservationRepository = reservationRepository;
            _carRepository = carRepository;
            _logger = logger;
        }

        // GET: api/reservations
        [HttpGet]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllAsync();
                _logger.LogInformation("Fetched {Count} reservations (Admin/Agent)", reservations.Count());

                var result = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    CarName = r.Car != null ? r.Car.Make + " " + r.Car.Model : "Unknown Car",
                    UserEmail = r.User?.Email ?? "Unknown",
                    PickupAt = r.PickupAt,
                    DropoffAt = r.DropoffAt,
                    PickupLocation = r.PickupLocation?.Name ?? "N/A",
                    DropoffLocation = r.DropoffLocation?.Name ?? "N/A",
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all reservations");
                return StatusCode(500, new { Message = "Error fetching reservations", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/reservations/user
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserReservations()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var reservations = await _reservationRepository.GetByUserIdAsync(userId);
                _logger.LogInformation("Fetched {Count} reservations for User {UserId}", reservations.Count(), userId);

                var result = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    CarName = r.Car != null ? r.Car.Make + " " + r.Car.Model : "Unknown Car",
                    UserEmail = r.User?.Email ?? "Unknown",
                    PickupAt = r.PickupAt,
                    DropoffAt = r.DropoffAt,
                    PickupLocation = r.PickupLocation?.Name ?? "N/A",
                    DropoffLocation = r.DropoffLocation?.Name ?? "N/A",
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservations for current user");
                return StatusCode(500, new { Message = "Error fetching user reservations", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // POST: api/reservations
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                if (dto.PickupAt.Date < DateTime.UtcNow.Date)
                    return BadRequest(new { Message = "Pickup date cannot be in the past" });

                if (dto.DropoffAt <= dto.PickupAt)
                    return BadRequest(new { Message = "Dropoff date must be after pickup date" });

                var days = (dto.DropoffAt.Date - dto.PickupAt.Date).Days;

                var car = await _carRepository.GetByIdAsync(dto.CarId);
                if (car == null) return NotFound(new { Message = "Car not found" });

                var totalPrice = days * car.PricePerDay;

                var reservation = new Reservation
                {
                    CarId = dto.CarId,
                    UserId = userId,
                    PickupAt = dto.PickupAt,
                    DropoffAt = dto.DropoffAt,
                    PickupLocationId = dto.PickupLocationId,
                    DropoffLocationId = dto.DropoffLocationId,
                    TotalPrice = totalPrice,
                    Status = "Pending"
                };

                await _reservationRepository.AddAsync(reservation);

                _logger.LogInformation("Reservation {ReservationId} created for Car {CarId} by User {UserId} (TotalPrice: {TotalPrice}, Days: {Days})",
                    reservation.Id, car.Id, userId, totalPrice, days);

                return Ok(new
                {
                    Message = "Reservation created successfully",
                    ReservationId = reservation.Id,
                    TotalPrice = totalPrice,
                    Days = days
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation for Car {CarId}", dto.CarId);
                return StatusCode(500, new { Message = "Error creating reservation", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // PUT: api/reservations/status
        [HttpPut("status")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateReservationStatus([FromBody] UpdateReservationStatusDto dto)
        {
            try
            {
                await _reservationRepository.UpdateStatusAsync(dto.ReservationId, dto.Status);
                _logger.LogInformation("Reservation {ReservationId} status updated to {Status}", dto.ReservationId, dto.Status);
                return Ok(new { Message = $"Reservation status updated to {dto.Status}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation {ReservationId} to {Status}", dto.ReservationId, dto.Status);
                return StatusCode(500, new { Message = "Error updating reservation status", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // DELETE: api/reservations/cancel/5
        [HttpDelete("cancel/{id}")]
        [Authorize(Roles = "User,Admin,Agent")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                await _reservationRepository.CancelAsync(id);
                _logger.LogInformation("Reservation {ReservationId} cancelled", id);
                return Ok(new { Message = "Reservation cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling reservation {ReservationId}", id);
                return StatusCode(500, new { Message = "Error cancelling reservation", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/reservations/admin/user/{userId}
        [HttpGet("admin/user/{userId}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetReservationsByUserIdForAdmin(string userId)
        {
            try
            {
                var reservations = await _reservationRepository.GetByUserIdForAdminAsync(userId);

                if (!reservations.Any())
                {
                    _logger.LogWarning("No reservations found for User {UserId}", userId);
                    return NotFound(new { Message = "No reservations found for this user" });
                }

                _logger.LogInformation("Fetched {Count} reservations for User {UserId} (Admin/Agent)", reservations.Count(), userId);

                var result = reservations.Select(r => new
                {
                    r.Id,
                    UserEmail = r.User?.Email ?? "Unknown",
                    Car = r.Car != null ? r.Car.Make + " " + r.Car.Model : "Unknown Car",
                    r.PickupAt,
                    r.DropoffAt,
                    PickupLocation = r.PickupLocation?.Name ?? "N/A",
                    DropoffLocation = r.DropoffLocation?.Name ?? "N/A",
                    r.TotalPrice,
                    r.Status
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservations for User {UserId} (Admin/Agent)", userId);
                return StatusCode(500, new { Message = "Error fetching reservations", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
