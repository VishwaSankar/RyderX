namespace RyderX_Server.Authentication
{
    public class UpdateProfileDTO
    {
        public string? DriverLicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }

    }
}
