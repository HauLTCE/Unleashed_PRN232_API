using AutoMapper;
using CartService.Dtos;
using CartService.Models;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }
        public async Task<Cart> CreateOrUpdateCartAsync(CreateCartDTO createCartDTO)
        {
            var existingCart = await _cartRepository.FindAsync((createCartDTO.UserId, createCartDTO.VariationId));
            if (existingCart != null)
            {
                existingCart.CartQuantity += createCartDTO.CartQuantity;
                _cartRepository.Update(existingCart);
                await _cartRepository.SaveAsync();
                return existingCart;
            }
            var newCart = _mapper.Map<Cart>(createCartDTO);
            await _cartRepository.CreateAsync(newCart);
            await _cartRepository.SaveAsync();
            return newCart;
        }

        public async Task<bool> DeleteCartAsync(Guid userId, int variationId)
        {
            var cart = await _cartRepository.FindAsync((userId, variationId));
            if (cart == null)
            {
                return false;
            }
            _cartRepository.Delete(cart);
            return await _cartRepository.SaveAsync(); 
        }

        public async Task<Cart?> GetCartAsync(Guid userId, int variationId)
        {
            return await _cartRepository.FindAsync((userId, variationId));
        }

        public async Task<IEnumerable<Cart>> GetCartsAsync()
        {
            return await _cartRepository.All().ToListAsync();
        }

        public async Task<IEnumerable<Cart>> GetCartsByUserIdAsync(Guid userId)
        {
            return await _cartRepository.GetCartsByUserIdAsync(userId);
        }

        public async Task<Cart?> UpdateCartAsync(Guid userId, int variationId, UpdateCartDTO updateCartDTO)
        {
            var cart = await _cartRepository.FindAsync((userId, variationId));
            if (cart == null)
            {
                return null;
            }
            cart.CartQuantity = updateCartDTO.CartQuantity;
            _cartRepository.Update(cart);
            await _cartRepository.SaveAsync();
            return cart;
        }
    }
}