namespace RyderX_Server.DTO.ReviewDTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
