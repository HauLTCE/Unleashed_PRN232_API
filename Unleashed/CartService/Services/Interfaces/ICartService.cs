using CartService.Dtos;
using CartService.DTOs;
using CartService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.Services.Interfaces
    {
        public interface ICartService
        {
        Task<List<GroupedCartDTO>> GetFormattedCartByUserIdAsync(Guid userId);
        Task AddToCartAsync(Guid userId, int variationId, int quantity);
        Task RemoveFromCartAsync(Guid userId, int variationId);
        Task RemoveAllFromCartAsync(Guid userId);
    }
    }
