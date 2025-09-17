using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.AnnouncementDTOs
{
    public class UpdateAnnouncementDto
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }

        [MaxLength(50)]
        public string? Audience { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public bool? IsActive { get; set; }
    }
}

