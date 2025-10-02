using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RyderX_Server.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;   

        [Required, MaxLength(100)]
        public string City { get; set; } = null!;

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(10)]
        public string? ZipCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        // Navigation - initialize to avoid null warnings
        public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
        public ICollection<Reservation> PickupReservations { get; set; } = new HashSet<Reservation>();
        public ICollection<Reservation> DropoffReservations { get; set; } = new HashSet<Reservation>();
    }
}
