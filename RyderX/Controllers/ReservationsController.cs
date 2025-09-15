using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.ReservationDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;
using System.Security.Claims;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationsController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        // GET: api/reservations
        [HttpGet]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllAsync();

                var result = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    CarName = r.Car.Make + " " + r.Car.Model,
                    UserEmail = r.User.Email,
                    PickupAt = r.PickupAt,
                    DropoffAt = r.DropoffAt,
                    PickupLocation = r.PickupLocation.Name,
                    DropoffLocation = r.DropoffLocation.Name,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching reservations", Details = ex.Message });
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
                return StatusCode(500, new { Message = "Error fetching user reservations", Details = ex.Message });
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

                var reservation = new Reservation
                {
                    CarId = dto.CarId,
                    UserId = userId,
                    PickupAt = dto.PickupAt,
                    DropoffAt = dto.DropoffAt,
                    PickupLocationId = dto.PickupLocationId,
                    DropoffLocationId = dto.DropoffLocationId,
                    TotalPrice = dto.TotalPrice,
                    Status = "Booked"
                };

                await _reservationRepository.AddAsync(reservation);
                return Ok(new { Message = "Reservation created successfully", ReservationId = reservation.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating reservation", Details = ex.Message });
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
                return Ok(new { Message = $"Reservation status updated to {dto.Status}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating reservation status", Details = ex.Message });
            }
        }

        // DELETE: api/reservations/cancel/5
        [HttpDelete("cancel/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                await _reservationRepository.CancelAsync(id);
                return Ok(new { Message = "Reservation cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error cancelling reservation", Details = ex.Message });
            }
        }
    }
}
