namespace RyderX_Server.DTO.ReservationDTOs
{
    public class CreateReservationDto
    {
        public int CarId { get; set; }
        public DateTime PickupAt { get; set; }
        public DateTime DropoffAt { get; set; }
        public int PickupLocationId { get; set; }
        public int DropoffLocationId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
