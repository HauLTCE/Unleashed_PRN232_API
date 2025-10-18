using CartService.DTOs.Wishlist;

namespace CartService.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<WishlistDTO> GetWishlistForUserAsync(Guid userId);
        Task AddToWishlistAsync(Guid userId, Guid productId);
        Task RemoveFromWishlistAsync(Guid userId, Guid productId);
        Task<bool> CheckIfProductInWishlistAsync(Guid userId, Guid productId);
    }
}