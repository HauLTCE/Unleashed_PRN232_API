using CartService.Dtos;
using CartService.Models;
using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            var carts = await _cartService.GetCartsAsync();
            return Ok(carts);
        }

        // GET: api/cart/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartsByUser(Guid userId)
        {
            var carts = await _cartService.GetCartsByUserIdAsync(userId);
            return Ok(carts);
        }

        // GET: api/cart/{userId}/{variationId}
        [HttpGet("{userId}/{variationId}")]
        public async Task<IActionResult> GetCart(Guid userId, int variationId)
        {
            var cart = await _cartService.GetCartAsync(userId, variationId);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateCart([FromBody] CreateCartDTO createCartDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cart = await _cartService.CreateOrUpdateCartAsync(createCartDTO);
            return Ok(cart);
        }

        // PUT: api/cart/{userId}/{variationId}
        [HttpPut("{userId}/{variationId}")]
        public async Task<IActionResult> UpdateCart(Guid userId, int variationId, [FromBody] UpdateCartDTO updateCartDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedCart = await _cartService.UpdateCartAsync(userId, variationId, updateCartDTO);
            if (updatedCart == null)
            {
                return NotFound();
            }
            return Ok(updatedCart);
        }

        // DELETE: api/cart/{userId}/{variationId}
        [HttpDelete("{userId}/{variationId}")]
        public async Task<IActionResult> DeleteCart(Guid userId, int variationId)
        {
            var deleted = await _cartService.DeleteCartAsync(userId, variationId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
