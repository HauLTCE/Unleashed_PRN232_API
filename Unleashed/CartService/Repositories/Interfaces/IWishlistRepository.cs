using CartService.Models;

namespace CartService.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetWishlistByUserIdAsync(Guid userId);
        Task AddAsync(Wishlist item);
        Task DeleteAsync(Wishlist item);
        Task<Wishlist?> FindAsync(Guid userId, Guid productId);
        Task<bool> ExistsAsync(Guid userId, Guid productId);
        Task<bool> SaveAsync();
    }
}