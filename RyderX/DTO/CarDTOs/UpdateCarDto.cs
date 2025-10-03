using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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

        [MaxLength(30)]
        public string? Category { get; set; }

        [MaxLength(20)]
        public string? FuelType { get; set; }

        [MaxLength(20)]
        public string? Transmission { get; set; }

        [Range(1, 15)]
        public int? Seats { get; set; }

        public string? Features { get; set; }

        // ✅ Image comes in multipart form like CreateCar
        public IFormFile? ImageFile { get; set; }
    }
}
