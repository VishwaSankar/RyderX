using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.CarDTOs
{
    public class CreateCarDto
    {
        [Required, MaxLength(50)]
        public string Make { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required, MaxLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required, Range(0, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        [Required]
        public int LocationId { get; set; }

        [MaxLength(30)]
        public string? Category { get; set; }   // e.g., Sedan, SUV, Hatchback

        [MaxLength(20)]
        public string? FuelType { get; set; }   // e.g., Petrol, Diesel, Electric

        [MaxLength(20)]
        public string? Transmission { get; set; } // e.g., Manual, Automatic

        [Range(1, 15)]
        public int Seats { get; set; }

        [MaxLength(200)]
        public string? Features { get; set; }   // e.g., GPS, Bluetooth, AC
    }
}
