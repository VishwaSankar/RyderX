using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.ExtraServiceDTOs
{
    public class UpdateExtraServiceDto
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
