using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? Category { get; set; }        // Sedan, SUV, Luxury etc.

        [MaxLength(20)]
        public string? FuelType { get; set; }        // Petrol / Diesel / Electric / Hybrid

        [MaxLength(20)]
        public string? Transmission { get; set; }    // Manual / Automatic

        [Range(1, 15)]
        public int Seats { get; set; }

        //public double? Mileage { get; set; }

        //public string? EngineType { get; set; }

        //public bool InsuranceIncluded { get; set; } = false;

        //[MaxLength(50)]
        //public string? FuelPolicy { get; set; }      // “Full to Full”, etc.

        public string? Features { get; set; }        // Comma-separated, e.g. "GPS, Bluetooth"

        //[Url]
        //public string? PhotoUrl { get; set; }

        //public string? ConditionDescription { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Foreign key to Location (where the car is parked / available)
        [Required]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; } = null!;

        // Navigation - initialize to avoid null warnings
        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}
