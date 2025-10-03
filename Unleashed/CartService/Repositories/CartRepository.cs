using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CartService.Data;
using CartService.Models;
using CartService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext _context;

        public CartRepository(CartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts.ToListAsync();
        }

        public async Task<Cart> GetByIdAsync(Guid userId, int variationId)
        {
            return await _context.Carts.FindAsync(userId, variationId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public void Update(Cart cart)
        {
            _context.Entry(cart).State = EntityState.Modified;
        }

        public void Remove(Cart cart)
        {
            _context.Carts.Remove(cart);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<bool> CartExistsAsync(Guid userId, int variationId)
        {
            return await _context.Carts.AnyAsync(e => e.UserId == userId && e.VariationId == variationId);
        }
    }
}