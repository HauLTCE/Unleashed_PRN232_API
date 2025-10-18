using AutoMapper;
using CartService.DTOs;
using CartService.DTOs.Cart;
using CartService.DTOs.Client;
using CartService.Models;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;
using System.Linq;

namespace CartService.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public CartService(
            ICartRepository cartRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<GroupedCartDTO>> GetFormattedCartByUserIdAsync(Guid userId)
        {
            var userCartItems = await _cartRepository.GetCartsByUserIdAsync(userId);
            var cartDtos = new List<CartItemDTO>();

            var productClient = _httpClientFactory.CreateClient("productservice");
            var inventoryClient = _httpClientFactory.CreateClient("inventoryservice");
            var discountClient = _httpClientFactory.CreateClient("discountservice");

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
                    var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/stock/{cartItem.VariationId}");
                    stockQuantity = stockDto?.StockQuantity ?? 0;
                }
                catch (HttpRequestException) { }

                SaleDTO? saleInfo = null;
                try
                {
                    saleInfo = await discountClient.GetFromJsonAsync<SaleDTO>($"api/sales/product/{variationDetails.ProductId}");
                }
                catch (HttpRequestException)
                {
                    saleInfo = null;
                }

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
                    StockQuantity = stockQuantity,
                    Sale = saleInfo
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
                var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/stock/{variationId}");
                stockQuantity = stockDto?.StockQuantity ?? 0;
            }
            catch (HttpRequestException)
            {
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
    }
}