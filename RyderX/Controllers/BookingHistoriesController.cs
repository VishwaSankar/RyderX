using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.BookingHistoryDTOs;
using RyderX_Server.Repositories.Interfaces;
using System.Security.Claims;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingHistoriesController : ControllerBase
    {
        private readonly IBookingHistoryRepository _bookingHistoryRepository;

        public BookingHistoriesController(IBookingHistoryRepository bookingHistoryRepository)
        {
            _bookingHistoryRepository = bookingHistoryRepository;
        }

        // GET: api/bookinghistories
        [HttpGet]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAllHistories()
        {
            try
            {
                var histories = await _bookingHistoryRepository.GetAllAsync();

                var result = histories.Select(h => new BookingHistoryDto
                {
                    Id = h.Id,
                    UserEmail = h.User?.Email ?? "Unknown",
                    CarMake = h.CarMake,
                    CarModel = h.CarModel,
                    CarLicensePlate = h.CarLicensePlate,
                    PickupAt = h.PickupAt,
                    DropoffAt = h.DropoffAt,
                    PickupLocation = h.PickupLocation,
                    DropoffLocation = h.DropoffLocation,
                    TotalPrice = h.TotalPrice,
                    Status = h.Status,
                    CreatedAt = h.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching booking histories", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/bookinghistories/user
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserHistories()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var histories = await _bookingHistoryRepository.GetByUserIdAsync(userId);

                var result = histories.Select(h => new BookingHistoryDto
                {
                    Id = h.Id,
                    UserEmail = h.User?.Email ?? "Unknown",
                    CarMake = h.CarMake,
                    CarModel = h.CarModel,
                    CarLicensePlate = h.CarLicensePlate,
                    PickupAt = h.PickupAt,
                    DropoffAt = h.DropoffAt,
                    PickupLocation = h.PickupLocation,
                    DropoffLocation = h.DropoffLocation,
                    TotalPrice = h.TotalPrice,
                    Status = h.Status,
                    CreatedAt = h.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching user booking histories", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // POST: api/bookinghistories
        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> CreateBookingHistory([FromBody] CreateBookingHistoryDto dto)
        {
            try
            {
                var history = await _bookingHistoryRepository.CreateFromReservationAsync(dto.ReservationId);

                return Ok(new
                {
                    Message = "Booking history recorded successfully",
                    HistoryId = history.Id,
                    FinalAmount = history.TotalPrice 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating booking history", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }
// GET: api/bookinghistories/agent/my
        [HttpGet("agent/my")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> GetAgentBookingHistories()
        {
            try
            {
                var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(agentId))
                    return Unauthorized(new { Message = "Invalid agent identity" });

                var histories = await _bookingHistoryRepository.GetAllAsync();

                var myHistories = histories
                    .Where(h => h.Reservation.Car != null && h.Reservation.Car.OwnerId == agentId)
                    .Select(h => new BookingHistoryDto
                    {
                        Id = h.Id,
                        UserEmail = h.User?.Email ?? "Unknown",
                        CarMake = h.CarMake,
                        CarModel = h.CarModel,
                        CarLicensePlate = h.CarLicensePlate,
                        PickupAt = h.PickupAt,
                        DropoffAt = h.DropoffAt,
                        PickupLocation = h.PickupLocation,
                        DropoffLocation = h.DropoffLocation,
                        TotalPrice = h.TotalPrice,
                        Status = h.Status,
                        CreatedAt = h.CreatedAt
                    });

                return Ok(myHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching agent booking histories", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

    }
}
