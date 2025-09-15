using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RyderX_Server.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; } = null!;

        [Required, Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(20)]
        public string PaymentMethod { get; set; } = null!;         // Card / UPI / etc.

        [Required]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? TransactionId { get; set; }        // From payment gateway
    }
}
