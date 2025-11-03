using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.Clients;
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
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        // POST: api/reviews
        [HttpPost]
        //[Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult<ReviewDto>> PostReview(CreateReviewDto reviewDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User ID claim not found in token.");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest("Invalid User ID format in token.");
            }


            //var userId = Guid.Parse("E43DFF5D-7CAC-45C7-A699-81B48BEB33EF"); //FOR TESTING, SINCE NO UI = NO ID

            reviewDto.UserId = userId;

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
        public async Task<ActionResult<bool>> CheckReviewExists([FromQuery] Guid productId, [FromQuery] Guid orderId, [FromQuery] Guid userId)
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
        //[Authorize]
        public async Task<IActionResult> PutReview(int id, UpdateReviewDto reviewDto)
        {
            var updateResult = await _reviewService.UpdateReviewAsync(id, reviewDto);
            if (!updateResult) return NotFound();
            return NoContent();
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var deleteResult = await _reviewService.DeleteReviewAsync(id);
            if (!deleteResult) return NotFound();
            return NoContent();
        }

        // GET: api/reviews/dashboard
        [HttpGet("dashboard")]
        //[Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> GetDashboardReviews([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            _logger.LogInformation("CALLED GET: api/reviews/dashboard");
            var result = await _reviewService.GetDashboardReviewsAsync(page, size);
            return Ok(result);
        }
    }
}