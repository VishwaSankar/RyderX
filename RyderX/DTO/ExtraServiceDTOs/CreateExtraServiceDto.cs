using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.ExtraServiceDTOs
{
    public class CreateExtraServiceDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
