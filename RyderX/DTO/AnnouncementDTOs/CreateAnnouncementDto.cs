using System.ComponentModel.DataAnnotations;

public class CreateAnnouncementDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Audience { get; set; }

    public DateTime? ExpiryDate { get; set; }
}