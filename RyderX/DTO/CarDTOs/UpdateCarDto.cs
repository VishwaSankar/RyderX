using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.CarDTOs
{
    public class UpdateCarDto
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Make { get; set; }

        [MaxLength(50)]
        public string? Model { get; set; }

        [Range(1900, 2100)]
        public int? Year { get; set; }

        [MaxLength(20)]
        public string? LicensePlate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PricePerDay { get; set; }

        public bool? IsAvailable { get; set; }
        public int? LocationId { get; set; }

        public string? Category { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int? Features { get; set; }

        // ✅ Optional
        public string? ImageUrl { get; set; }
    }
}
