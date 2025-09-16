using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.PaymentDTOs
{
    public class CreatePaymentDto
    {
        public int ReservationId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, MaxLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // Card / UPI / Cash

        public string? TransactionId { get; set; } // from gateway
    }
}
