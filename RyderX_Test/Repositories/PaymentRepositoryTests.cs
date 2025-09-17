using NUnit.Framework;
using RyderX_Server.Authentication;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;
using System;
using System.Threading.Tasks;

namespace RyderX_Tests.Repositories
{
    public class PaymentRepositoryTests
    {
        [Test]
        public async Task AddPayment_ShouldAddPayment()
        {
            var context = TestHelper.GetInMemoryDbContext("PaymentDb");
            var reservationRepo = new ReservationRepository(context);
            var paymentRepo = new PaymentRepository(context);

            // 🔹 Seed Location
            var location = new Location { Name = "Main Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            // 🔹 Seed Car
            var car = new Car
            {
                Make = "Hyundai",
                Model = "i20",
                Year = 2022,
                LicensePlate = "TN03",
                PricePerDay = 200,
                LocationId = location.Id
            };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            // 🔹 Seed User
            var user = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@example.com",
                UserName = "user1@example.com"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // 🔹 Seed Reservation
            var reservation = new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(3),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 600,
                Status = "Pending"
            };
            await reservationRepo.AddAsync(reservation);

            // 🔹 Now create Payment (ReservationId must exist!)
            var payment = new Payment
            {
                ReservationId = reservation.Id,
                Amount = 600,
                PaymentMethod = "Card",
                TransactionId = "TXN12345",
                PaidAt = DateTime.UtcNow
            };

            await paymentRepo.AddAsync(payment);

            // Act
            var result = await paymentRepo.GetByIdAsync(payment.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.EqualTo(600));
            Assert.That(result.ReservationId, Is.EqualTo(reservation.Id));
        }
    }
}