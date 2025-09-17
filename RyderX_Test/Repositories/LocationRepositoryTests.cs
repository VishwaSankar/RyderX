using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;

namespace RyderX_Tests.Repositories
{
    public class LocationRepositoryTests
    {
        [Test]
        public async Task AddLocation_ShouldAddLocation()
        {
            var context = TestHelper.GetInMemoryDbContext("LocationDb_Add");
            var repo = new LocationRepository(context);

            var location = new Location { Name = "Airport Branch", City = "Chennai" };
            await repo.AddAsync(location);

            var locations = await repo.GetAllAsync();

            Assert.That(locations.Count(), Is.EqualTo(1));
            Assert.That(locations.First().Name, Is.EqualTo("Airport Branch"));
        }

        [Test]
        public async Task GetAllLocations_ShouldReturnAllLocations()
        {
            var context = TestHelper.GetInMemoryDbContext("LocationDb_All");
            var repo = new LocationRepository(context);

            await repo.AddAsync(new Location { Name = "Branch1", City = "Chennai" });
            await repo.AddAsync(new Location { Name = "Branch2", City = "Madurai" });

            var locations = await repo.GetAllAsync();

            Assert.That(locations.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetById_ShouldReturnLocationWithCars()
        {
            var context = TestHelper.GetInMemoryDbContext("LocationDb_ById");
            var repo = new LocationRepository(context);

            var location = new Location { Name = "Central", City = "Coimbatore" };
            await repo.AddAsync(location);

            var car = new Car
            {
                Make = "Honda",
                Model = "City",
                Year = 2021,
                LicensePlate = "TN01",
                PricePerDay = 200,
                LocationId = location.Id
            };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(location.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Cars.Count, Is.EqualTo(1));
            Assert.That(result.Cars.First().Model, Is.EqualTo("City"));
        }

        

        
    }
}
