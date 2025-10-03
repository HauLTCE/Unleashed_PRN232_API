using CartService.Dtos;
using CartService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.Services.Interfaces
    {
        public interface ICartService
        {
            Task<IEnumerable<Cart>> GetCartsAsync();
            Task<IEnumerable<Cart>> GetCartsByUserIdAsync(Guid userId);
            Task<Cart?> GetCartAsync(Guid userId, int variationId);
            Task<Cart> CreateOrUpdateCartAsync(CreateCartDTO createCartDTO);
            Task<Cart?> UpdateCartAsync(Guid userId, int variationId, UpdateCartDTO updateCartDTO);
            Task<bool> DeleteCartAsync(Guid userId, int variationId);
        }
    }
