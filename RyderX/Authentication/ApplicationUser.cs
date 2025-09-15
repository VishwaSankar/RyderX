using Microsoft.AspNetCore.Identity;
using RyderX_Server.Models;

namespace RyderX_Server.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }

        // Navigation
        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public ICollection<BookingHistory> BookingHistories { get; set; } = new HashSet<BookingHistory>();

    }
}
