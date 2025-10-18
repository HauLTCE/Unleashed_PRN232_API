using AutoMapper;
using CartService.Dtos;
using CartService.DTOs;
using CartService.Models;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
// BƯỚC 1: Thêm using cho các DTO client mới tạo
using CartService.DTOs.ClientDTOs;
using CartService.DTOs.ClientDTO;

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
            // var discountClient = _httpClientFactory.CreateClient("discountservice"); // Tạm thời chưa dùng

            foreach (var cartItem in userCartItems)
            {
                // BƯỚC 2: Sử dụng DTO cục bộ (ProductVariationDTO) để hứng dữ liệu
                ProductVariationDTO variationDetails = null;
                try
                {
                    variationDetails = await productClient.GetFromJsonAsync<ProductVariationDTO>($"api/variations/{cartItem.VariationId}");
                }
                catch (HttpRequestException) { /* Bỏ qua nếu service lỗi */ }

                if (variationDetails == null) continue;

                // BƯỚC 3: Sử dụng DTO cục bộ (InventoryStockDTO) để hứng dữ liệu
                int stockQuantity = 0;
                try
                {
                    // API của InventoryService phải trả về JSON có trường stockQuantity
                    var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/stock/{cartItem.VariationId}");
                    stockQuantity = stockDto?.StockQuantity ?? 0;
                }
                catch (HttpRequestException) { /* Mặc định là 0 nếu service lỗi */ }

                var variationDto = new VariationDTO // Đây là DTO để hiển thị trong giỏ hàng
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
                    ProductName = variationDetails.ProductName, // << BƯỚC 4: Lưu ProductName
                    Quantity = cartItem.CartQuantity ?? 0,
                    StockQuantity = stockQuantity,
                    // Sale = ... // Logic lấy sale sẽ thêm sau
                });
            }

            // BƯỚC 5: GroupBy theo ProductName đã được lưu trong CartItemDTO
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
            // Đảm bảo tên client nhất quán ("inventoryservice" thay vì "InventoryClient")
            var inventoryClient = _httpClientFactory.CreateClient("inventoryservice");
            int stockQuantity = 0;
            try
            {
                // Gọi API và hứng kết quả bằng DTO của mình
                var stockDto = await inventoryClient.GetFromJsonAsync<InventoryDTO>($"api/stock/{variationId}");

                // Lấy giá trị từ thuộc tính đã được đổi tên
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

        // Các phương thức Remove không thay đổi
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