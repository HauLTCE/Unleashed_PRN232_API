using CartService.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.Services.Interfaces
{
    public interface ICartService
    {
        Task<ActionResult<IEnumerable<CartDTO>>> GetCarts();
        Task<ActionResult<CartDTO>> GetCart(Guid userId, int variationId);
        Task<IActionResult> PutCart(Guid userId, int variationId, UpdateCartDTO updateCartDTO);
        Task<ActionResult<CartDTO>> PostCart(CreateCartDTO createCartDTO);
        Task<IActionResult> DeleteCart(Guid userId, int variationId);
    }
}