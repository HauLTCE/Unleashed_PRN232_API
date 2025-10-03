using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CartService.Dtos;
using CartService.Models;
using CartService.Repositories;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartService.Services
{
    public class CartServices : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartServices(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<CartDTO>>> GetCarts()
        {
            var carts = await _cartRepository.GetAllAsync();
            return new OkObjectResult(_mapper.Map<IEnumerable<CartDTO>>(carts));
        }

        public async Task<ActionResult<CartDTO>> GetCart(Guid userId, int variationId)
        {
            var cart = await _cartRepository.GetByIdAsync(userId, variationId);

            if (cart == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(_mapper.Map<CartDTO>(cart));
        }

        public async Task<IActionResult> PutCart(Guid userId, int variationId, UpdateCartDTO updateCartDTO)
        {
            var cartFromRepo = await _cartRepository.GetByIdAsync(userId, variationId);
            if (cartFromRepo == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(updateCartDTO, cartFromRepo);
            _cartRepository.Update(cartFromRepo);

            try
            {
                await _cartRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _cartRepository.CartExistsAsync(userId, variationId))
                {
                    return new NotFoundResult();
                }
                else
                {
                    throw;
                }
            }

            return new NoContentResult();
        }

        public async Task<ActionResult<CartDTO>> PostCart(CreateCartDTO createCartDTO)
        {
            var cart = _mapper.Map<Cart>(createCartDTO);

            try
            {
                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await _cartRepository.CartExistsAsync(cart.UserId, cart.VariationId))
                {
                    return new ConflictResult();
                }
                else
                {
                    throw;
                }
            }
            var cartDto = _mapper.Map<CartDTO>(cart);

            return new CreatedAtActionResult("GetCart", "Carts", new { userId = cartDto.UserId, variationId = cartDto.VariationId }, cartDto);
        }

        public async Task<IActionResult> DeleteCart(Guid userId, int variationId)
        {
            var cart = await _cartRepository.GetByIdAsync(userId, variationId);
            if (cart == null)
            {
                return new NotFoundResult();
            }

            _cartRepository.Remove(cart);
            await _cartRepository.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}