using NUnit.Framework;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;
using System.Linq;
using System.Threading.Tasks;

namespace RyderX_Tests.Repositories
{
    public class LocationRepositoryTests
    {
        [Test]
        public async Task AddLocation_ShouldAddLocation()
        {
            var context = TestHelper.GetInMemoryDbContext("LocationDb");
            var repo = new LocationRepository(context);

            var location = new Location { Name = "Airport Branch", City = "Chennai" };
            await repo.AddAsync(location);

            var locations = await repo.GetAllAsync();

            Assert.That(locations.Count(), Is.EqualTo(1));
            Assert.That(locations.First().Name, Is.EqualTo("Airport Branch"));
        }
    }
}