namespace RyderX_Server.DTO.ReservationDTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime PickupAt { get; set; }
        public DateTime DropoffAt { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
