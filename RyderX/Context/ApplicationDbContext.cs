using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RyderX_Server.Authentication;
using RyderX_Server.Models;

namespace RyderX_Server.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets for your domain models
        public DbSet<Car> Cars { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ExtraService> ExtraServices { get; set; }
        public DbSet<BookingHistory> BookingHistories { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>()
                .HasOne(c => c.Location)
                .WithMany(l => l.Cars)
                .HasForeignKey(c => c.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.PickupLocation)
                .WithMany(l => l.PickupReservations)
                .HasForeignKey(r => r.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.DropoffLocation)
                .WithMany(l => l.DropoffReservations)
                .HasForeignKey(r => r.DropoffLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reservation>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Reservation)
                .HasForeignKey<Payment>(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(rv => rv.Car)
                .WithMany(c => c.Reviews)
                .HasForeignKey(rv => rv.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(rv => rv.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(rv => rv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BookingHistory>()
                 .HasOne(b => b.Reservation)
                 .WithMany()
                 .HasForeignKey(b => b.ReservationId)
                 .OnDelete(DeleteBehavior.Restrict);

            // BookingHistory → User (1:M)
            builder.Entity<BookingHistory>()
                .HasOne(b => b.User)
                .WithMany(u => u.BookingHistories)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
