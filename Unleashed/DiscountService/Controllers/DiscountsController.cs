using DiscountService.DTOs;
using DiscountService.Models;
using DiscountService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // GET: api/discount
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscounts()
        {
            var discounts = await _discountService.GetDiscountsAsync();
            return Ok(discounts);
        }

        // GET: api/discount/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Discount>> GetDiscountById(int id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }

        // POST: api/discount
        [HttpPost]
        public async Task<ActionResult<Discount>> CreateDiscount([FromBody] CreateDiscountDto createDiscountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var discount = await _discountService.CreateDiscountAsync(createDiscountDto);
            return CreatedAtAction(nameof(GetDiscountById), new { id = discount.DiscountId }, discount);
        }

        // PUT: api/discount/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Discount>> UpdateDiscount(int id, [FromBody] UpdateDiscountDto updateDiscountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedDiscount = await _discountService.UpdateDiscountAsync(id, updateDiscountDto);
            if (updatedDiscount == null)
            {
                return NotFound();
            }
            return Ok(updatedDiscount);
        }

        // DELETE: api/discount/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteDiscount(int id)
        {
            var deleted = await _discountService.DeleteDiscountAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/discount/statuses
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<object>>> GetDiscountStatuses()
        {
            var statuses = await _discountService.GetDiscountStatusesAsync();
            return Ok(statuses);
        }

        // GET: api/discount/types
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<object>>> GetDiscountTypes()
        {
            var types = await _discountService.GetDiscountTypesAsync();
            return Ok(types);
        }
    }
}
