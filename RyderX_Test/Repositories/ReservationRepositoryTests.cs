using NUnit.Framework;
using RyderX_Server.Authentication;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;
using System.Linq;
using System.Threading.Tasks;

namespace RyderX_Tests.Repositories
{
    public class ReservationRepositoryTests
    {
        [Test]
        public async Task AddReservation_ShouldAddReservation_AndMarkCarUnavailable()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_Add");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "City Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Honda", Model = "Civic", Year = 2021, LicensePlate = "TN02", PricePerDay = 150, LocationId = location.Id, IsAvailable = true };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user1", Email = "user1@example.com", UserName = "user1@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var reservation = new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(2),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 300,
                Status = "Booked"
            };

            await repo.AddAsync(reservation);

            var reservations = await repo.GetAllAsync();
            var updatedCar = await context.Cars.FindAsync(car.Id);

            Assert.That(reservations.Count(), Is.EqualTo(1));
            Assert.That(reservations.First().Status, Is.EqualTo("Booked"));
            Assert.That(updatedCar!.IsAvailable, Is.False, "Car should be unavailable after booking");
        }

        [Test]
        public void AddReservation_ShouldThrow_WhenCarUnavailable()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_AddFail");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            context.Locations.Add(location);
            context.SaveChanges();

            var car = new Car { Make = "Ford", Model = "EcoSport", Year = 2019, LicensePlate = "TN03", PricePerDay = 90, LocationId = location.Id, IsAvailable = false };
            context.Cars.Add(car);
            context.SaveChanges();

            var user = new ApplicationUser { Id = "userX", Email = "x@example.com", UserName = "x@example.com" };
            context.Users.Add(user);
            context.SaveChanges();

            var reservation = new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(1),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 200,
                Status = "Booked"
            };

            Assert.ThrowsAsync<Exception>(async () => await repo.AddAsync(reservation));
        }

        [Test]
        public async Task CancelReservation_ShouldMarkAsCancelled_FreeCar_AndLogHistory()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_Cancel");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Nissan", Model = "Micra", Year = 2018, LicensePlate = "TN06", PricePerDay = 150, LocationId = location.Id, IsAvailable = true };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user5", Email = "user5@example.com", UserName = "user5@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var reservation = new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(3),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 450,
                Status = "Booked"
            };
            await repo.AddAsync(reservation);

            await repo.CancelAsync(reservation.Id);

            var updated = await repo.GetByIdAsync(reservation.Id);
            var updatedCar = await context.Cars.FindAsync(car.Id);
            var history = context.BookingHistories.FirstOrDefault();

            Assert.That(updated.Status, Is.EqualTo("Cancelled"));
            Assert.That(updatedCar!.IsAvailable, Is.True, "Car should be freed after cancel");
            Assert.That(history, Is.Not.Null);
            Assert.That(history.Status, Is.EqualTo("Cancelled"));
        }

        [Test]
        public async Task UpdateStatus_ShouldCompleteReservation_FreeCar_AndLogHistory()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_UpdateStatus");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Hyundai", Model = "i20", Year = 2020, LicensePlate = "TN07", PricePerDay = 200, LocationId = location.Id, IsAvailable = true };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user6", Email = "user6@example.com", UserName = "user6@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var reservation = new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(2),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 400,
                Status = "Booked"
            };
            await repo.AddAsync(reservation);

            await repo.UpdateStatusAsync(reservation.Id, "Completed");

            var updated = await repo.GetByIdAsync(reservation.Id);
            var updatedCar = await context.Cars.FindAsync(car.Id);
            var history = context.BookingHistories.FirstOrDefault();

            Assert.That(updated.Status, Is.EqualTo("Completed"));
            Assert.That(updatedCar!.IsAvailable, Is.True, "Car should be freed after completion");
            Assert.That(history, Is.Not.Null);
            Assert.That(history.Status, Is.EqualTo("Completed"));
        }

        [Test]
        public async Task GetActiveReservations_ShouldReturnOnlyBooked()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_Active");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car1 = new Car { Make = "Tesla", Model = "Model 3", Year = 2022, LicensePlate = "TN08", PricePerDay = 800, LocationId = location.Id, IsAvailable = true };
            var car2 = new Car { Make = "Tesla", Model = "Model Y", Year = 2022, LicensePlate = "TN09", PricePerDay = 900, LocationId = location.Id, IsAvailable = true };
            await context.Cars.AddRangeAsync(car1, car2);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user7", Email = "user7@example.com", UserName = "user7@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Reservation { CarId = car1.Id, UserId = user.Id, PickupAt = DateTime.Now, DropoffAt = DateTime.Now.AddDays(1), PickupLocationId = location.Id, DropoffLocationId = location.Id, TotalPrice = 800, Status = "Booked" });
            await repo.AddAsync(new Reservation { CarId = car2.Id, UserId = user.Id, PickupAt = DateTime.Now, DropoffAt = DateTime.Now.AddDays(2), PickupLocationId = location.Id, DropoffLocationId = location.Id, TotalPrice = 900, Status = "Cancelled" });

            var results = await repo.GetActiveReservationsAsync(user.Id);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().Status, Is.EqualTo("Booked"));
        }

        [Test]
        public async Task GetByUserIdForAdmin_ShouldReturnWithUserInfo()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_Admin");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Toyota", Model = "Fortuner", Year = 2021, LicensePlate = "TN10", PricePerDay = 1000, LocationId = location.Id, IsAvailable = true };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user8", Email = "user8@example.com", UserName = "user8@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(3),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 3000,
                Status = "Booked"
            });

            var results = await repo.GetByUserIdForAdminAsync(user.Id);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().User.Email, Is.EqualTo("user8@example.com"));
        }
    }
}
