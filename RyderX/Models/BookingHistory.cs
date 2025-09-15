using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RyderX_Server.Authentication;

namespace RyderX_Server.Models
{
    public class BookingHistory
    {
        public int Id { get; set; }

        // FK to Reservation
        [Required]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation Reservation { get; set; } = null!;

        // FK to User (who owns the booking) - denormalized for display
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        // Car info (snapshot)
        [Required, MaxLength(50)]
        public string CarMake { get; set; } = null!;

        [Required, MaxLength(50)]
        public string CarModel { get; set; } = null!;

        [Required, MaxLength(20)]
        public string CarLicensePlate { get; set; } = null!;

        // Pickup / Dropoff
        [Required]
        public DateTime PickupAt { get; set; }

        [Required]
        public DateTime DropoffAt { get; set; }

        [Required, MaxLength(100)]
        public string PickupLocation { get; set; } = null!;

        [Required, MaxLength(100)]
        public string DropoffLocation { get; set; } = null!;

        // Price & Status
        [Required, Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]

        public decimal TotalPrice { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Booked";  // Booked, Cancelled, Completed

        // Metadata
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
