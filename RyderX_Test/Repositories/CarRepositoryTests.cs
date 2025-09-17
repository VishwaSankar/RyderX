using NUnit.Framework;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;
using System.Linq;
using System.Threading.Tasks;

namespace RyderX_Tests.Repositories
{
    public class CarRepositoryTests
    {
        [Test]
        public async Task AddCar_ShouldAddCar()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_Add");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Test Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car
            {
                Make = "Toyota",
                Model = "Corolla",
                Year = 2020,
                LicensePlate = "TN01",
                PricePerDay = 100,
                LocationId = location.Id,
                IsAvailable = true
            };

            await repo.AddAsync(car);
            var cars = await repo.GetAllAsync();

            Assert.That(cars.Count(), Is.EqualTo(1));
            Assert.That(cars.First().Make, Is.EqualTo("Toyota"));
        }

        [Test]
        public async Task GetAllCars_ShouldReturnAllCars()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_GetAll");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Car { Make = "Honda", Model = "City", Year = 2021, LicensePlate = "TN02", PricePerDay = 120, LocationId = location.Id, IsAvailable = true });
            await repo.AddAsync(new Car { Make = "Ford", Model = "EcoSport", Year = 2019, LicensePlate = "TN03", PricePerDay = 90, LocationId = location.Id, IsAvailable = true });

            var cars = await repo.GetAllAsync();
            Assert.That(cars.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_ShouldReturnCar()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_GetById");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Suzuki", Model = "Swift", Year = 2022, LicensePlate = "TN04", PricePerDay = 110, LocationId = location.Id, IsAvailable = true };
            await repo.AddAsync(car);

            var result = await repo.GetByIdAsync(car.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo("Swift"));
        }

        [Test]
        public async Task GetByLocation_ShouldReturnCarsAtThatLocation()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_GetByLocation");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Car { Make = "BMW", Model = "X1", Year = 2020, LicensePlate = "TN05", PricePerDay = 500, LocationId = location.Id, IsAvailable = true });
            await repo.AddAsync(new Car { Make = "Audi", Model = "A4", Year = 2021, LicensePlate = "TN06", PricePerDay = 600, LocationId = location.Id, IsAvailable = true });

            var result = await repo.GetByLocationAsync(location.Id);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task SearchCars_ShouldReturnMatchingCars()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_Search");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Car { Make = "Tesla", Model = "Model 3", Category = "EV", Year = 2022, LicensePlate = "TN07", PricePerDay = 800, LocationId = location.Id, IsAvailable = true });
            await repo.AddAsync(new Car { Make = "Hyundai", Model = "i20", Category = "Hatchback", Year = 2021, LicensePlate = "TN08", PricePerDay = 200, LocationId = location.Id, IsAvailable = false });

            var result = await repo.SearchAsync("Tesla", null, null, null, null, null, true);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Model, Is.EqualTo("Model 3"));
        }

        [Test]
        public async Task UpdateCar_ShouldModifyCarDetails()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_Update");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Nissan", Model = "Micra", Year = 2018, LicensePlate = "TN09", PricePerDay = 150, LocationId = location.Id, IsAvailable = true };
            await repo.AddAsync(car);

            car.PricePerDay = 180;
            await repo.UpdateAsync(car);

            var updated = await repo.GetByIdAsync(car.Id);
            Assert.That(updated.PricePerDay, Is.EqualTo(180));
        }

        [Test]
        public async Task DeleteCar_ShouldRemoveCar()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_Delete");
            var repo = new CarRepository(context);

            var location = new Location { Name = "Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Kia", Model = "Seltos", Year = 2021, LicensePlate = "TN10", PricePerDay = 300, LocationId = location.Id, IsAvailable = true };
            await repo.AddAsync(car);

            await repo.DeleteAsync(car.Id);
            var cars = await repo.GetAllAsync();

            Assert.That(cars.Count(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteCar_ShouldThrowIfNotFound()
        {
            var context = TestHelper.GetInMemoryDbContext("CarDb_DeleteNotFound");
            var repo = new CarRepository(context);

            Assert.ThrowsAsync<Exception>(async () => await repo.DeleteAsync(999));
        }
    }
}
