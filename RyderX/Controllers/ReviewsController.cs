using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.ReviewDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;
using System.Security.Claims;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET: api/reviews/car/5
        [HttpGet("car/{carId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsForCar(int carId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByCarIdAsync(carId);

                if (!reviews.Any())
                    return NotFound(new { Message = "No reviews found for this car" });

                var result = reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    CarName = r.Car != null ? r.Car.Make + " " + r.Car.Model : "Unknown Car",
                    UserEmail = r.User?.Email ?? "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error fetching reviews for car {carId}", Details = ex.Message });
            }
        }

        // GET: api/reviews/user
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserReviews()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var reviews = await _reviewRepository.GetReviewsByUserIdAsync(userId);

                if (!reviews.Any())
                    return NotFound(new { Message = "No reviews found for this user" });

                var result = reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    CarName = r.Car != null ? r.Car.Make + " " + r.Car.Model : "Unknown Car",
                    UserEmail = r.User?.Email ?? "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching user reviews", Details = ex.Message });
            }
        }

        // POST: api/reviews
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Message = "Invalid user identity" });

                var review = new Review
                {
                    CarId = dto.CarId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _reviewRepository.AddAsync(review);
                return Ok(new { Message = "Review added successfully", ReviewId = review.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error adding review", Details = ex.Message });
            }
        }

        // DELETE: api/reviews/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewRepository.DeleteAsync(id);
                return Ok(new { Message = "Review deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error deleting review {id}", Details = ex.Message });
            }
        }

        // GET: api/reviews/average/car/5
        [HttpGet("average/car/{carId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAverageRatingForCar(int carId)
        {
            try
            {
                var avg = await _reviewRepository.GetAverageRatingAsync(carId);
                return Ok(new { CarId = carId, AverageRating = avg });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error calculating average rating for car {carId}", Details = ex.Message });
            }
        }
    }
}
