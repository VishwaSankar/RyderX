namespace RyderX_Server.DTO.CarDTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; }
        public string? Category { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int Seats { get; set; }
        public string? Features { get; set; }
        public string LocationName { get; set; } = string.Empty;

        // ✅ NEW: Owner Info
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty; // e.g. "Vishwa S"
    }
}
