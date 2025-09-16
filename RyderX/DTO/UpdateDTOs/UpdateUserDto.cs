namespace RyderX_Server.DTO.UpdateDTOs
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

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
