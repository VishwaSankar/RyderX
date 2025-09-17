using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;   // e.g., "Festive Offer: 20% Off!"

        [Required, MaxLength(1000)]
        public string Message { get; set; } = string.Empty; // detailed announcement

        [MaxLength(50)]
        public string? Audience { get; set; }  // e.g., "All", "Users", "Agents", "Admins"

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; } // when the announcement expires

        public bool IsActive { get; set; } = true; // control visibility
    }
}
