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

        public IQueryable<Cart> All()
        {
            return _context.Carts.AsQueryable();
        }

        public async Task<bool> CreateAsync(Cart entity)
        {
            try
            {
                await _context.Carts.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Cart entity)
        {
            try
            {
                _context.Carts.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Cart?> FindAsync((Guid, int) id)
        {

             var (userId, variationId) = id;
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.VariationId == variationId);
        }

        public async Task<IEnumerable<Cart>> GetCartsByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsAny((Guid, int) id)
        {
            var (userId, variationId) = id;
            return await _context.Carts
                .AnyAsync(c => c.UserId == userId && c.VariationId == variationId);

        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Cart entity)
        {
            try
            {
                _context.Carts.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}