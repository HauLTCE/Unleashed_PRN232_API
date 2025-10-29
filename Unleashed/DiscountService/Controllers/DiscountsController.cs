using DiscountService.DTOs;
using DiscountService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DiscountService.Controllers
{
    [ApiController]
    [Route("api/discounts")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // === Endpoint cho Admin/Staff ===

        // POST: api/discounts
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdDiscount = await _discountService.CreateDiscountAsync(createDto);
                return CreatedAtAction(nameof(GetDiscountById), new { id = createdDiscount.DiscountId }, createdDiscount);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/discounts/{id}
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] UpdateDiscountDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedDiscount = await _discountService.UpdateDiscountAsync(id, updateDto);
            if (updatedDiscount == null)
            {
                return NotFound();
            }
            return Ok(updatedDiscount);
        }

        // DELETE: api/discounts/{id}
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var deleted = await _discountService.DeleteDiscountAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/discounts
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts(
            [FromQuery] string? search,
            [FromQuery] int? statusId,
            [FromQuery] int? typeId,
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            var result = await _discountService.GetAllDiscountsAsync(search, statusId, typeId, page, size);
            return Ok(result);
        }

        // POST: api/discounts/{discountId}/users
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpPost("{discountId:int}/users")]
        public async Task<IActionResult> AddUsersToDiscount(int discountId, [FromBody] List<string> userIds)
        {
            try
            {
                await _discountService.AddUsersToDiscountAsync(discountId, userIds);
                return Ok("Users added to discount successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/discounts/{discountId}/users?userId={userId}
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpDelete("{discountId:int}/users")]
        public async Task<IActionResult> RemoveUserFromDiscount(int discountId, [FromQuery] string userId)
        {
            await _discountService.RemoveUserFromDiscountAsync(discountId, userId);
            return NoContent();
        }

        // GET: api/discounts/{discountId}/users
        // [Authorize(Roles = "ADMIN,STAFF")]
        [HttpGet("{discountId:int}/users")]
        public async Task<IActionResult> GetUsersByDiscountId(int discountId)
        {
            var result = await _discountService.GetUsersByDiscountIdAsync(discountId);
            return Ok(result);
        }

        // === Endpoint cho Customer ===

        // GET: api/discounts/me
        // [Authorize(Roles = "CUSTOMER")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyDiscounts(
            [FromQuery] string? search,
            [FromQuery] int? statusId,
            [FromQuery] int? typeId,
            [FromQuery] int page = 0,
            [FromQuery] int size = 9,
            // SỬA LỖI Ở DÒNG DƯỚI ĐÂY: Thêm "= null"
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortOrder = "asc")
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _discountService.GetDiscountsForUserAsync(userId, search, statusId, typeId, page, size, sortBy, sortOrder);
            return Ok(result);
        }

        // GET: api/discounts/me/{discountId}
        // [Authorize(Roles = "CUSTOMER")]
        [HttpGet("me/{discountId:int}")]
        public async Task<IActionResult> GetMyDiscountById(int discountId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var discount = await _discountService.GetDiscountForUserByIdAsync(discountId, userId);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }

        // GET: api/discounts/best-for-checkout?cartTotal={total}
        // [Authorize(Roles = "CUSTOMER")]
        [HttpGet("best-for-checkout")]
        public async Task<IActionResult> GetBestDiscountsForCheckout([FromQuery] decimal cartTotal)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var bestDiscounts = await _discountService.GetBestDiscountsForCheckoutAsync(userId, cartTotal);
            return Ok(bestDiscounts);
        }


        // === Endpoint công khai (Public) ===

        // GET: api/discounts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDiscountById(int id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }

        // GET: api/discounts/statuses
        [HttpGet("statuses")]
        public async Task<IActionResult> GetAllDiscountStatuses()
        {
            var statuses = await _discountService.GetAllStatusesAsync();
            return Ok(statuses);
        }

        // GET: api/discounts/types
        [HttpGet("types")]
        public async Task<IActionResult> GetAllDiscountTypes()
        {
            var types = await _discountService.GetAllTypesAsync();
            return Ok(types);
        }

        // GET: api/discounts/check-code?code={code}
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckDiscountCodeExists([FromQuery] string code)
        {
            var exists = await _discountService.IsDiscountCodeExistsAsync(code);
            return Ok(exists);
        }

        // GET: api/discounts/check-user-discount?discountCode={code}&subTotal={total}
        // [Authorize(Roles = "CUSTOMER")]
        [HttpGet("check-user-discount")]
        public async Task<IActionResult> CheckUserDiscount([FromQuery] string discountCode, [FromQuery] decimal subTotal)
        {
            var result = await _discountService.CheckUserDiscountAsync(discountCode, subTotal, User);
            return Ok(result);
        }

        // === Helper Method ===
        private string? GetCurrentUserId()
        {
            // Lấy User ID từ claim trong JWT token
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}