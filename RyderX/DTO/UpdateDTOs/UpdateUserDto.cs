using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.DTO.UpdateDTOs
{
    public class UpdateUserDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? LicenseNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LicenseExpiry { get; set; }

        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }
}
