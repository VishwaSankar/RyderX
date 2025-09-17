using NUnit.Framework;
using RyderX_Server.Authentication;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;


namespace RyderX_Tests.Repositories
{
    public class ReservationRepositoryTests
    {
        [Test]
        public async Task AddReservation_ShouldAddReservation()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb");
            var repo = new ReservationRepository(context);

            // 🔹 Seed Location
            var location = new Location { Name = "City Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            // 🔹 Seed Car (linked to Location)
            var car = new Car
            {
                Make = "Honda",
                Model = "Civic",
                Year = 2021,
                LicensePlate = "TN02",
                PricePerDay = 150,
                LocationId = location.Id
            };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            // 🔹 Seed User (ApplicationUser) if required
            var user = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@example.com",
                UserName = "user1@example.com"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // 🔹 Now Reservation can reference Car, Location, User
            var reservation = new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(2),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 300,
                Status = "Pending"
            };

            await repo.AddAsync(reservation);

            var reservations = await repo.GetAllAsync();

            Assert.That(reservations.Count(), Is.EqualTo(1));
            Assert.That(reservations.First().Status, Is.EqualTo("Pending"));
        }
    }
}