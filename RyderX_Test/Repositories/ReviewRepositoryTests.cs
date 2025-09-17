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
            var context = TestHelper.GetInMemoryDbContext("ReviewDb_Add");
            var repo = new ReviewRepository(context);

            var location = new Location { Name = "Central Branch", City = "Chennai" };
            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();

            var car = new Car { Make = "Audi", Model = "A4", Year = 2020, LicensePlate = "TN04", PricePerDay = 300, LocationId = location.Id };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();

            var user = new ApplicationUser { Id = "user1", Email = "user1@example.com", UserName = "user1@example.com" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var review = new Review { CarId = car.Id, UserId = user.Id, Rating = 5, Comment = "Great ride!" };
            await repo.AddAsync(review);

            var reviews = await repo.GetReviewsByCarIdAsync(car.Id);

            Assert.That(reviews.Count(), Is.EqualTo(1));
            Assert.That(reviews.First().Rating, Is.EqualTo(5));
            Assert.That(reviews.First().UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public async Task DeleteReview_ShouldRemoveReview()
        {
            var context = TestHelper.GetInMemoryDbContext("ReviewDb_Delete");
            var repo = new ReviewRepository(context);

            var car = new Car { Make = "Honda", Model = "Civic", Year = 2019, LicensePlate = "TN05", PricePerDay = 200 };
            var user = new ApplicationUser { Id = "user2", Email = "user2@example.com", UserName = "user2@example.com" };
            await context.Cars.AddAsync(car);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var review = new Review { CarId = car.Id, UserId = user.Id, Rating = 4, Comment = "Nice car" };
            await repo.AddAsync(review);

            await repo.DeleteAsync(review.Id);

            var reviews = await repo.GetReviewsByCarIdAsync(car.Id);
            Assert.That(reviews.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetAverageRating_ShouldReturnCorrectAverage()
        {
            var context = TestHelper.GetInMemoryDbContext("ReviewDb_Avg");
            var repo = new ReviewRepository(context);

            var car = new Car { Make = "BMW", Model = "X1", Year = 2021, LicensePlate = "TN06", PricePerDay = 500 };
            var user = new ApplicationUser { Id = "user3", Email = "user3@example.com", UserName = "user3@example.com" };
            await context.Cars.AddAsync(car);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Review { CarId = car.Id, UserId = user.Id, Rating = 4, Comment = "Good" });
            await repo.AddAsync(new Review { CarId = car.Id, UserId = user.Id, Rating = 2, Comment = "Average" });

            var avg = await repo.GetAverageRatingAsync(car.Id);

            Assert.That(avg, Is.EqualTo(3)); // (4 + 2) / 2
        }

        [Test]
        public async Task GetReviewsByCarId_ShouldReturnReviewsForThatCar()
        {
            var context = TestHelper.GetInMemoryDbContext("ReviewDb_ByCar");
            var repo = new ReviewRepository(context);

            var car = new Car { Make = "Tesla", Model = "Model 3", Year = 2022, LicensePlate = "TN07", PricePerDay = 800 };
            var user = new ApplicationUser { Id = "user4", Email = "user4@example.com", UserName = "user4@example.com" };
            await context.Cars.AddAsync(car);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Review { CarId = car.Id, UserId = user.Id, Rating = 5, Comment = "Amazing EV" });

            var reviews = await repo.GetReviewsByCarIdAsync(car.Id);

            Assert.That(reviews.Count(), Is.EqualTo(1));
            Assert.That(reviews.First().Car.Make, Is.EqualTo("Tesla"));
            Assert.That(reviews.First().User.Email, Is.EqualTo("user4@example.com"));
        }

        [Test]
        public async Task GetReviewsByUserId_ShouldReturnReviewsForThatUser()
        {
            var context = TestHelper.GetInMemoryDbContext("ReviewDb_ByUser");
            var repo = new ReviewRepository(context);

            var car = new Car { Make = "Toyota", Model = "Fortuner", Year = 2020, LicensePlate = "TN08", PricePerDay = 1000 };
            var user = new ApplicationUser { Id = "user5", Email = "user5@example.com", UserName = "user5@example.com" };
            await context.Cars.AddAsync(car);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repo.AddAsync(new Review { CarId = car.Id, UserId = user.Id, Rating = 3, Comment = "Decent SUV" });

            var reviews = await repo.GetReviewsByUserIdAsync(user.Id);

            Assert.That(reviews.Count(), Is.EqualTo(1));
            Assert.That(reviews.First().User.Email, Is.EqualTo("user5@example.com"));
            Assert.That(reviews.First().Car.Model, Is.EqualTo("Fortuner"));
        }
    }
}
