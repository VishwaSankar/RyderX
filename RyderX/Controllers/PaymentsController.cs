using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.PaymentDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;
using System.Security.Claims;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null) return NotFound(new { Message = "Payment not found" });

                var dto = new PaymentDto
                {
                    Id = payment.Id,
                    ReservationId = payment.ReservationId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaidAt = payment.PaidAt,
                    TransactionId = payment.TransactionId
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error fetching payment {id}", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/payments/reservation/{reservationId}
        [HttpGet("reservation/{reservationId}")]
        [Authorize(Roles = "User,Admin,Agent")]
        public async Task<IActionResult> GetPaymentByReservation(int reservationId)
        {
            try
            {
                var payment = await _paymentRepository.GetByReservationIdAsync(reservationId);
                if (payment == null) return NotFound(new { Message = "No payment for this reservation" });

                var dto = new PaymentDto
                {
                    Id = payment.Id,
                    ReservationId = payment.ReservationId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaidAt = payment.PaidAt,
                    TransactionId = payment.TransactionId
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching payment", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/payments/user
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserPayments()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var payments = await _paymentRepository.GetByUserIdAsync(userId);

                var result = payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    ReservationId = p.ReservationId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    TransactionId = p.TransactionId
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching user payments", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // POST: api/payments
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var payment = new Payment
                {
                    ReservationId = dto.ReservationId,
                    PaymentMethod = "Manual", // or Stripe later
                    TransactionId = Guid.NewGuid().ToString(),
                    PaidAt = DateTime.UtcNow
                };

                await _paymentRepository.AddAsync(payment);

                return Ok(new
                {
                    Message = "Payment created successfully",
                    PaymentId = payment.Id,
                    ReservationId = payment.ReservationId,
                    FinalAmount = payment.Amount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating payment", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/payments/admin/user/{userId}
        [HttpGet("admin/user/{userId}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetPaymentsByUserIdForAdmin(string userId)
        {
            try
            {
                var payments = await _paymentRepository.GetByUserIdForAdminAsync(userId);

                if (!payments.Any())
                    return NotFound(new { Message = "No payments found for this user" });

                var result = payments.Select(p => new
                {
                    p.Id,
                    UserEmail = p.Reservation?.User?.Email ?? "Unknown",
                    ReservationId = p.ReservationId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    TransactionId = p.TransactionId
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching payments", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/payments/agent/my
        [HttpGet("agent/my")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> GetAgentPayments()
        {
            try
            {
                var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(agentId))
                    return Unauthorized(new { Message = "Invalid agent identity" });

                var payments = await _paymentRepository.GetByOwnerIdAsync(agentId);

                var result = payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    ReservationId = p.ReservationId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaidAt = p.PaidAt,
                    TransactionId = p.TransactionId
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching agent payments", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
