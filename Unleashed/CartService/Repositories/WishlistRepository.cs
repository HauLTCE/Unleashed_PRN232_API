using CartService.Data;
using CartService.Models;
using CartService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly CartDbContext _context;

        public WishlistRepository(CartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetWishlistByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(Wishlist item)
        {
            await _context.Wishlists.AddAsync(item);
        }

        public Task DeleteAsync(Wishlist item)
        {
            _context.Wishlists.Remove(item);
            return Task.CompletedTask;
        }

        public async Task<Wishlist?> FindAsync(Guid userId, Guid productId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid productId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}