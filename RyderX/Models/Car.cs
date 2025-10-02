using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RyderX_Server.Authentication; 
namespace RyderX_Server.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Make { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Model { get; set; } = null!;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required, MaxLength(20)]
        public string LicensePlate { get; set; } = null!;

        [Required, Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerDay { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        [MaxLength(30)]
        public string? Category { get; set; }

        [MaxLength(20)]
        public string? FuelType { get; set; }

        [MaxLength(20)]
        public string? Transmission { get; set; }

        [Range(1, 15)]
        public int Seats { get; set; }

        public string? Features { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // ✅ NEW: Car Owner
        public string? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }

        // ✅ Placeholder for future image upload
        public string? ImageUrl { get; set; }

        // Foreign key to Location
        [Required]
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        public Location Location { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}
