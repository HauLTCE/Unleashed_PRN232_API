using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CartService.Dtos;
using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDTO>>> GetCarts()
        {
            return await _cartService.GetCarts();
        }

        // GET: api/Carts/{userId}/{variationId}
        [HttpGet("{userId}/{variationId}")]
        public async Task<ActionResult<CartDTO>> GetCart(Guid userId, int variationId)
        {
            return await _cartService.GetCart(userId, variationId);
        }

        // PUT: api/Carts/{userId}/{variationId}
        [HttpPut("{userId}/{variationId}")]
        public async Task<IActionResult> PutCart(Guid userId, int variationId, UpdateCartDTO updateCartDTO)
        {
            return await _cartService.PutCart(userId, variationId, updateCartDTO);
        }

        // POST: api/Carts
        [HttpPost]
        public async Task<ActionResult<CartDTO>> PostCart(CreateCartDTO createCartDTO)
        {
            return await _cartService.PostCart(createCartDTO);
        }

        // DELETE: api/Carts/{userId}/{variationId}
        [HttpDelete("{userId}/{variationId}")]
        public async Task<IActionResult> DeleteCart(Guid userId, int variationId)
        {
            return await _cartService.DeleteCart(userId, variationId);
        }
    }
}