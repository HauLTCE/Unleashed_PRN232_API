using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                              ?? User.FindFirst("sub"); // thêm fallback
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid in the token.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetFormattedCartByUserIdAsync(userId);
                return Ok(cart);
            }
            catch (Exception e)
            {
                return BadRequest($"Error fetching user cart: {e.Message}");
            }
        }

        [HttpPost("{variationId}")]
        public async Task<IActionResult> AddToCart(int variationId, [FromQuery] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                var userId = GetCurrentUserId();
                await _cartService.AddToCartAsync(userId, variationId, quantity);
                return Ok("Successfully added item to the cart.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{variationId}")]
        public async Task<IActionResult> RemoveFromCart(int variationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _cartService.RemoveFromCartAsync(userId, variationId);
                return Ok("Successfully removed item from cart.");
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest($"Error removing item from cart: {e.Message}");
            }
        }

        [HttpDelete("all")]
        public async Task<IActionResult> RemoveAllFromCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _cartService.RemoveAllFromCartAsync(userId);
                return Ok("Successfully removed all items from cart.");
            }
            catch (Exception e)
            {
                return BadRequest($"Error removing all items from cart: {e.Message}");
            }
        }
    }
}