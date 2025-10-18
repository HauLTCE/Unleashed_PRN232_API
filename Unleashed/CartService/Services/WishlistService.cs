using CartService.DTOs.Client;
using CartService.DTOs.Wishlist;
using CartService.Models;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;

namespace CartService.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public WishlistService(IWishlistRepository wishlistRepository, IHttpClientFactory httpClientFactory)
        {
            _wishlistRepository = wishlistRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WishlistDTO> GetWishlistForUserAsync(Guid userId)
        {
            var wishlistItems = await _wishlistRepository.GetWishlistByUserIdAsync(userId);
            var productClient = _httpClientFactory.CreateClient("productservice");
            var productInfoTasks = new List<Task<WishlistProductInfoDTO?>>();

            foreach (var item in wishlistItems)
            {
                productInfoTasks.Add(FetchProductInfoAsync(productClient, item.ProductId));
            }

            var productInfos = await Task.WhenAll(productInfoTasks);

            return new WishlistDTO
            {
                UserId = userId,
                Products = productInfos.Where(p => p != null).ToList()!
            };
        }

        private async Task<WishlistProductInfoDTO?> FetchProductInfoAsync(HttpClient client, Guid productId)
        {
            try
            {
                // This assumes productservice has an endpoint like /api/products/for-wishlist/{productId}
                // that returns the required fields (Id, Name, Image, Status).
                return await client.GetFromJsonAsync<WishlistProductInfoDTO>($"api/products/for-wishlist/{productId}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task AddToWishlistAsync(Guid userId, Guid productId)
        {
            if (await _wishlistRepository.ExistsAsync(userId, productId))
            {
                throw new InvalidOperationException("Product already exists in the wishlist.");
            }

            var wishlistItem = new Wishlist { UserId = userId, ProductId = productId };
            await _wishlistRepository.AddAsync(wishlistItem);
            await _wishlistRepository.SaveAsync();
        }

        public async Task RemoveFromWishlistAsync(Guid userId, Guid productId)
        {
            var itemToRemove = await _wishlistRepository.FindAsync(userId, productId);
            if (itemToRemove == null)
            {
                throw new KeyNotFoundException("Product not found in the wishlist.");
            }

            await _wishlistRepository.DeleteAsync(itemToRemove);
            await _wishlistRepository.SaveAsync();
        }

        public async Task<bool> CheckIfProductInWishlistAsync(Guid userId, Guid productId)
        {
            return await _wishlistRepository.ExistsAsync(userId, productId);
        }
    }
}