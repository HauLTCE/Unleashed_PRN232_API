using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing, invalid, or empty in the token.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var userId = GetCurrentUserId();
                var wishlist = await _wishlistService.GetWishlistForUserAsync(userId);
                return Ok(wishlist);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching wishlist: {ex.Message}");
            }
        }

        [HttpPost("{productId:guid}")]
        public async Task<IActionResult> AddToWishlist(Guid productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _wishlistService.AddToWishlistAsync(userId, productId);
                return Ok("Product successfully added to wishlist.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _wishlistService.RemoveFromWishlistAsync(userId, productId);
                return Ok("Product successfully removed from wishlist.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("check/{productId:guid}")]
        public async Task<IActionResult> CheckWishlist(Guid productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                bool isInWishlist = await _wishlistService.CheckIfProductInWishlistAsync(userId, productId);
                return Ok(isInWishlist);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking wishlist: {ex.Message}");
            }
        }
    }
}