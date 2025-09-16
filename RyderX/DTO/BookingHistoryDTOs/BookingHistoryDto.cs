namespace RyderX_Server.DTO.BookingHistoryDTOs
{
    public class BookingHistoryDto
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public string CarMake { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public string CarLicensePlate { get; set; } = string.Empty;

        public DateTime PickupAt { get; set; }
        public DateTime DropoffAt { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
