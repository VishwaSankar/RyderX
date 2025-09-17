using NUnit.Framework;
using RyderX_Server.Authentication;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Implementation;
using System.Linq;
using System.Threading.Tasks;

namespace RyderX_Tests.Repositories
{
    public class ReviewRepositoryTests
    {
        [Test]
        public async Task AddReview_ShouldAddReview()
        {
            var context = TestHelper.GetInMemoryDbContext("ReviewDb");
            var repo = new ReviewRepository(context);

            // Seed Location
            var location = new Location { Name = "Central Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            // Seed Car
            var car = new Car
            {
                Make = "Audi",
                Model = "A4",
                Year = 2020,
                LicensePlate = "TN04",
                PricePerDay = 300,
                LocationId = location.Id
            };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            // Seed User
            var user = new ApplicationUser
            {
                Id = "user3",
                Email = "user3@example.com",
                UserName = "user3@example.com"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Create Review (Car + User required)
            var review = new Review
            {
                CarId = car.Id,
                UserId = user.Id,
                Rating = 5,
                Comment = "Great ride!"
            };
            await repo.AddAsync(review);

            // Act
            var reviews = await repo.GetReviewsByCarIdAsync(car.Id);

            // Assert
            Assert.That(reviews.Count(), Is.EqualTo(1));
            Assert.That(reviews.First().Rating, Is.EqualTo(5));
            Assert.That(reviews.First().UserId, Is.EqualTo(user.Id));
        }
    }
}