using AutoMapper;
using CartService.DTOs;
using CartService.DTOs.Cart;
using CartService.DTOs.Client;
using CartService.Models;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CartService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<GroupedCartDTO>> GetFormattedCartByUserIdAsync(Guid userId)
        {
            var userCartItems = await _cartRepository.GetCartsByUserIdAsync(userId);
            var cartDtos = new List<CartItemDTO>();

            var productClient = _httpClientFactory.CreateClient("productservice");
            var inventoryClient = _httpClientFactory.CreateClient("inventoryservice");

            foreach (var cartItem in userCartItems)
            {
                ProductVariationDTO? variationDetails = null;
                try
                {
                    variationDetails = await productClient.GetFromJsonAsync<ProductVariationDTO>($"api/variations/{cartItem.VariationId}");
                }
                catch (HttpRequestException) { }

                if (variationDetails == null) continue;

                int stockQuantity = 0;
                try
                {
                    var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/stockvariations/get-stock-by-variation/{cartItem.VariationId}");
                    stockQuantity = stockDto?.TotalQuantity ?? 0;
                }
                catch (HttpRequestException) { }

                var variationDto = new VariationDTO
                {
                    Id = variationDetails.VariationId,
                    VariationPrice = variationDetails.VariationPrice ?? 0,
                    VariationImage = variationDetails.VariationImage,
                    ColorName = variationDetails.ColorName,
                    SizeName = variationDetails.SizeName
                };

                cartDtos.Add(new CartItemDTO
                {
                    Variation = variationDto,
                    ProductName = variationDetails.ProductName,
                    Quantity = cartItem.CartQuantity ?? 0,
                    StockQuantity = stockQuantity
                });
            }

            var groupedCart = cartDtos
                .GroupBy(dto => dto.ProductName)
                .Select(group => new GroupedCartDTO
                {
                    ProductName = group.Key,
                    Items = group.ToList()
                })
                .ToList();

            return groupedCart;
        }

        public async Task AddToCartAsync(Guid userId, int variationId, int quantity)
        {
            var inventoryClient = _httpClientFactory.CreateClient("inventoryservice");
            int stockQuantity = 0;
            try
            {
                var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/stockvariations/get-stock-by-variation/{variationId}");
                stockQuantity = stockDto?.TotalQuantity ?? 0;
            }
            // ++ CHỈNH SỬA: Bắt exception và ghi log chi tiết
            catch (HttpRequestException ex)
            {
                // Ghi lại lỗi gốc để giúp debug dễ dàng hơn
                _logger.LogError(ex, "Failed to connect to InventoryService for variationId {VariationId}. Base Address: {BaseAddress}",
                    variationId, inventoryClient.BaseAddress);

                // Ném ra exception với thông điệp quen thuộc cho client
                throw new InvalidOperationException("Could not verify stock for this item. Inventory service is unavailable.");
            }

            var existingCartItem = await _cartRepository.FindAsync((userId, variationId));
            int currentCartQuantity = existingCartItem?.CartQuantity ?? 0;
            int totalQuantity = currentCartQuantity + quantity;

            if (totalQuantity > stockQuantity)
            {
                throw new InvalidOperationException($"Adding {quantity} item(s) would exceed available stock ({totalQuantity}/{stockQuantity}).");
            }

            if (existingCartItem == null)
            {
                var newCart = new Cart { UserId = userId, VariationId = variationId, CartQuantity = quantity };
                await _cartRepository.CreateAsync(newCart);
            }
            else
            {
                existingCartItem.CartQuantity = totalQuantity;
                _cartRepository.Update(existingCartItem);
            }
            await _cartRepository.SaveAsync();
        }

        public async Task RemoveFromCartAsync(Guid userId, int variationId)
        {
            var cartItem = await _cartRepository.FindAsync((userId, variationId));
            if (cartItem == null) throw new KeyNotFoundException("Item not found in cart.");
            _cartRepository.Delete(cartItem);
            await _cartRepository.SaveAsync();
        }

        public async Task RemoveAllFromCartAsync(Guid userId)
        {
            var userCarts = await _cartRepository.GetCartsByUserIdAsync(userId);
            if (!userCarts.Any()) throw new InvalidOperationException("No items in cart to remove.");
            await _cartRepository.DeleteAllByUserIdAsync(userId);
        }
    

    public async Task UpdateCartQuantityAsync(Guid userId, int variationId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                // Nếu số lượng mới là 0 hoặc âm, hãy xóa sản phẩm
                await RemoveFromCartAsync(userId, variationId);
                return;
            }

            var inventoryClient = _httpClientFactory.CreateClient("inventoryservice");
            int stockQuantity = 0;
            try
            {
                var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/StockVariations/get-stock-by-variation/{variationId}");
                stockQuantity = stockDto?.TotalQuantity ?? 0;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to InventoryService for variationId {VariationId}.", variationId);
                throw new InvalidOperationException("Could not verify stock for this item. Inventory service is unavailable.");
            }

            if (newQuantity > stockQuantity)
            {
                throw new InvalidOperationException($"Cannot update quantity to {newQuantity}. Only {stockQuantity} item(s) available in stock.");
            }

            var existingCartItem = await _cartRepository.FindAsync((userId, variationId));
            if (existingCartItem == null)
            {
                throw new KeyNotFoundException("Item not found in cart to update.");
            }

            existingCartItem.CartQuantity = newQuantity;
            _cartRepository.Update(existingCartItem);
            await _cartRepository.SaveAsync();
        }
    }
}
