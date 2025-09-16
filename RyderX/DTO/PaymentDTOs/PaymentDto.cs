namespace RyderX_Server.DTO.PaymentDTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
        public string? TransactionId { get; set; }
    }
}
