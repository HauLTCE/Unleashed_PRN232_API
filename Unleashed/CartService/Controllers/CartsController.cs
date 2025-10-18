using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    //[Authorize] // BƯỚC 1: Bắt buộc tất cả các request đến controller này phải được xác thực (đăng nhập)
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // BƯỚC 2: Tạo một phương thức private để lấy UserId từ token một cách an toàn
        // Điều này đảm bảo người dùng chỉ có thể thao tác trên giỏ hàng của chính mình.
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                // Ném lỗi nếu không tìm thấy hoặc không hợp lệ, [Authorize] sẽ trả về 401 Unauthorized
                throw new UnauthorizedAccessException("User ID claim is missing or invalid in the token.");
            }
            return userId;
        }

        /// <summary>
        /// Lấy giỏ hàng chi tiết của người dùng đang đăng nhập.
        /// </summary>
        // GET: api/cart
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
                // Trả về lỗi 400 Bad Request nếu có vấn đề xảy ra
                return BadRequest($"Error fetching user cart: {e.Message}");
            }
        }

        /// <summary>
        /// Thêm một sản phẩm vào giỏ hàng của người dùng đang đăng nhập.
        /// </summary>
        /// <param name="variationId">ID của biến thể sản phẩm.</param>
        /// <param name="quantity">Số lượng cần thêm (gửi trong body của request).</param>
        // POST: api/cart/{variationId}
        [HttpPost("{variationId}")]
        public async Task<IActionResult> AddToCart(int variationId, [FromBody] int quantity)
        {
            // Kiểm tra đầu vào cơ bản
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
                // Trả về thông báo lỗi cụ thể (ví dụ: lỗi hết hàng)
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng của người dùng đang đăng nhập.
        /// </summary>
        /// <param name="variationId">ID của biến thể sản phẩm cần xóa.</param>
        // DELETE: api/cart/{variationId}
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
                return NotFound(e.Message); // Trả về 404 Not Found nếu không tìm thấy sản phẩm
            }
            catch (Exception e)
            {
                return BadRequest($"Error removing item from cart: {e.Message}");
            }
        }

        /// <summary>
        /// Xóa tất cả sản phẩm khỏi giỏ hàng của người dùng đang đăng nhập.
        /// </summary>
        // DELETE: api/cart/all
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