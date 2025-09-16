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
            var context = TestHelper.GetInMemoryDbContext("CarDb");
            var repo = new CarRepository(context);

            // 🔹 Seed required Location (foreign key constraint)
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
    }
}
