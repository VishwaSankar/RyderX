using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.ReviewDTOs
{
    public class CreateReviewDto
    {
        public int CarId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
