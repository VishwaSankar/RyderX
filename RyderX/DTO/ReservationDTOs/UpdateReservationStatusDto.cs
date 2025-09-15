namespace RyderX_Server.DTO.ReservationDTOs
{
    public class UpdateReservationStatusDto
    {
        public int ReservationId { get; set; }
        public string Status { get; set; } = "Booked"; // Booked / Cancelled / Completed
    }
}
