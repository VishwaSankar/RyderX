using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RyderX_Server.Authentication;

namespace RyderX_Server.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        // FK to Car
        [Required]
        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Car? Car { get; set; }

        // FK to User
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime PickupAt { get; set; }

        [Required]
        public DateTime DropoffAt { get; set; }

        [Required, Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]

        public decimal TotalPrice { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Booked";   // Booked / InProgress / Cancelled / Completed

        // Pickup and Dropoff Locations (FKs)
        [Required]
        public int PickupLocationId { get; set; }

        [ForeignKey("PickupLocationId")]
        public Location? PickupLocation { get; set; }

        [Required]
        public int DropoffLocationId { get; set; }

        [ForeignKey("DropoffLocationId")]
        public Location? DropoffLocation { get; set; }

        // Payment navigation (one-to-one): Payment may be null until paid
        public Payment? Payment { get; set; }
    }
}
