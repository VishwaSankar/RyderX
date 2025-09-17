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
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_Add");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "City Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

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

            var user = new ApplicationUser
            {
                Id = "user1",
                Email = "user1@example.com",
                UserName = "user1@example.com"
            };
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
                Status = "Pending"
            };

            await repo.AddAsync(reservation);
            var reservations = await repo.GetAllAsync();

            Assert.That(reservations.Count(), Is.EqualTo(1));
            Assert.That(reservations.First().Status, Is.EqualTo("Pending"));
        }

        [Test]
        public async Task GetById_ShouldReturnReservation()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_GetById");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Ford", Model = "EcoSport", Year = 2019, LicensePlate = "TN03", PricePerDay = 90, LocationId = location.Id };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user2", Email = "user2@example.com", UserName = "user2@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

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
            await repo.AddAsync(reservation);

            var result = await repo.GetByIdAsync(reservation.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("Booked"));
        }

        [Test]
        public async Task GetByUserId_ShouldReturnReservationsForUser()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_GetByUserId");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "BMW", Model = "X1", Year = 2020, LicensePlate = "TN04", PricePerDay = 500, LocationId = location.Id };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user3", Email = "user3@example.com", UserName = "user3@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(2),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 600,
                Status = "Booked"
            });

            var results = await repo.GetByUserIdAsync(user.Id);

            Assert.That(results.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByCarId_ShouldReturnReservationsForCar()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_GetByCarId");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Audi", Model = "A4", Year = 2021, LicensePlate = "TN05", PricePerDay = 600, LocationId = location.Id };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user4", Email = "user4@example.com", UserName = "user4@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Reservation
            {
                CarId = car.Id,
                UserId = user.Id,
                PickupAt = DateTime.Now,
                DropoffAt = DateTime.Now.AddDays(1),
                PickupLocationId = location.Id,
                DropoffLocationId = location.Id,
                TotalPrice = 600,
                Status = "Booked"
            });

            var results = await repo.GetByCarIdAsync(car.Id);

            Assert.That(results.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task CancelReservation_ShouldMarkAsCancelled_AndLogHistory()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_Cancel");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Nissan", Model = "Micra", Year = 2018, LicensePlate = "TN06", PricePerDay = 150, LocationId = location.Id };
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
            var history = context.BookingHistories.FirstOrDefault();

            Assert.That(updated.Status, Is.EqualTo("Cancelled"));
            Assert.That(history, Is.Not.Null);
            Assert.That(history.Status, Is.EqualTo("Cancelled"));
        }

        [Test]
        public async Task UpdateStatus_ShouldUpdateToCompleted_AndLogHistory()
        {
            var context = TestHelper.GetInMemoryDbContext("ReservationDb_UpdateStatus");
            var repo = new ReservationRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Hyundai", Model = "i20", Year = 2020, LicensePlate = "TN07", PricePerDay = 200, LocationId = location.Id };
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
            var history = context.BookingHistories.FirstOrDefault();

            Assert.That(updated.Status, Is.EqualTo("Completed"));
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

            var car = new Car { Make = "Tesla", Model = "Model 3", Year = 2022, LicensePlate = "TN08", PricePerDay = 800, LocationId = location.Id };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user7", Email = "user7@example.com", UserName = "user7@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Reservation { CarId = car.Id, UserId = user.Id, PickupAt = DateTime.Now, DropoffAt = DateTime.Now.AddDays(1), PickupLocationId = location.Id, DropoffLocationId = location.Id, TotalPrice = 800, Status = "Booked" });
            await repo.AddAsync(new Reservation { CarId = car.Id, UserId = user.Id, PickupAt = DateTime.Now, DropoffAt = DateTime.Now.AddDays(2), PickupLocationId = location.Id, DropoffLocationId = location.Id, TotalPrice = 1600, Status = "Cancelled" });

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

            var car = new Car { Make = "Toyota", Model = "Fortuner", Year = 2021, LicensePlate = "TN09", PricePerDay = 1000, LocationId = location.Id };
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
