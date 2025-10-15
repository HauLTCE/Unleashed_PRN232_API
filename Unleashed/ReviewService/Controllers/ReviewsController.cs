using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.DTOs.Review;
using ReviewService.Services.Interfaces;
using System.Security.Claims;

namespace ReviewService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: api/reviews
        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult<ReviewDto>> PostReview(CreateReviewDto reviewDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            reviewDto.UserId = userId; // Ensure the DTO uses the authenticated user's ID

            var createdReview = await _reviewService.CreateReviewAsync(reviewDto, userId);
            return CreatedAtAction(nameof(GetReview), new { id = createdReview.ReviewId }, createdReview);
        }

        // GET: api/reviews/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProductId(Guid productId, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            Guid? currentUserId = User.Identity?.IsAuthenticated == true
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
                : null;

            var result = await _reviewService.GetAllReviewsByProductIdAsync(productId, page, size, currentUserId);
            return Ok(result);
        }

        // GET: api/reviews/comments/{commentId}/replies
        [HttpGet("comments/{commentId}/replies")]
        public async Task<IActionResult> GetCommentReplies(int commentId, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var result = await _reviewService.GetRepliesForCommentAsync(commentId, page, size);
            return Ok(result);
        }

        // GET: api/reviews/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewsByUserId(Guid userId, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var result = await _reviewService.GetReviewsByUserIdAsync(userId, page, size);
            return Ok(result);
        }

        // GET: api/reviews/eligibility
        [HttpGet("eligibility")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CheckEligibility([FromQuery] Guid productId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _reviewService.GetReviewEligibilityAsync(productId, userId);
            return Ok(result);
        }

        // GET: api/reviews/check-exists
        [HttpGet("check-exists")]
        public async Task<ActionResult<bool>> CheckReviewExists([FromQuery] Guid productId, [FromQuery] string orderId, [FromQuery] Guid userId)
        {
            var exists = await _reviewService.CheckReviewExistsAsync(productId, orderId, userId);
            return Ok(exists);
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null) return NotFound();
            return Ok(review);
        }

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        [Authorize] // Or more specific roles
        public async Task<IActionResult> PutReview(int id, UpdateReviewDto reviewDto)
        {
            var updateResult = await _reviewService.UpdateReviewAsync(id, reviewDto);
            if (!updateResult) return NotFound();
            return NoContent();
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var deleteResult = await _reviewService.DeleteReviewAsync(id);
            if (!deleteResult) return NotFound();
            return NoContent();
        }
    }
}