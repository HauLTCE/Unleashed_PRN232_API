using AutoMapper;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using OrderService.Clients.IClients;
using OrderService.Dtos;
using OrderService.DTOs.ResponesDtos;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Interfaces;
using ProductService.Clients.IClients;
using System.Security.Cryptography;

namespace OrderService.Services
{
    public class OrderServices : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepo _orderStatusRepository;
        private readonly IOrderVariationRepo _orderVariationSingleRepository;
        private readonly IProductApiClient _productApiClient;
        private readonly IInventoryApiClient _inventoryApiClient;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderServices> _logger;
        public OrderServices(IOrderRepository orderRepository, IOrderStatusRepo orderStatusRepository, IOrderVariationRepo orderVariationSingleRepository, IProductApiClient productApiClient,IInventoryApiClient inventoryApiClient, IMapper mapper, ILogger<OrderServices> logger)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderVariationSingleRepository = orderVariationSingleRepository;
            _productApiClient = productApiClient;
            _inventoryApiClient = inventoryApiClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // === PHASE 1: VALIDATION (Get data from other APIs) ===

            // Get all variation IDs to fetch data in one batch
            var variationIds = createOrderDto.OrderVariations
                .Select(v => v.VariationId)
                .ToList();

          
                var variationDetails = await _productApiClient.GetDetailsByIdsAsync(variationIds)
                ?? throw new Exception($"Variation {variationIds} not found.");

            if (variationDetails.Count() != variationIds.Count)
            {
                var foundIds = variationDetails.Select(v => v.VariationId).ToHashSet();
                var missingIds = variationIds.Where(id => !foundIds.Contains(id));

                throw new KeyNotFoundException(
                    $"Some variation IDs were not found: {string.Join(", ", missingIds)}"
                );
            }

            // === PHASE 2: STOCK CHECK ===
            var variationLookup = variationDetails.ToDictionary(v => v.VariationId);
            var requestedQty = createOrderDto.OrderVariations.ToDictionary(v => v.VariationId, v => v.Quantity);
            var stockLevels = await _inventoryApiClient.GetStockByIdsAsync(variationIds);
            if (!stockLevels.Any() || stockLevels.Contains(null)) throw new Exception($"Not found enough stock");
            decimal totalAmount = 0;

            foreach (var stockItem in stockLevels)
            {
                if (!requestedQty.TryGetValue(stockItem.VariationId, out var qty) || qty <= 0)
                    continue;

                if (qty > stockItem.TotalQuantity)
                    throw new InvalidOperationException(
                        $"Not enough stock for variation {stockItem.VariationId}. Requested: {qty}, Available: {stockItem.TotalQuantity}");

                if (!variationLookup.TryGetValue(stockItem.VariationId, out var variation))
                    throw new KeyNotFoundException($"Price not found for variation {stockItem.VariationId}");

                totalAmount += qty!.Value * variation.VariationPrice;
            }

            var order = _mapper.Map<Order>(createOrderDto) ?? throw new Exception("Mapping failed");


            order.OrderId = Guid.NewGuid();
            order.OrderDate = DateTimeOffset.UtcNow;
            order.OrderTrackingNumber = GenerateTrackingNumber() ?? throw new Exception("Tracking number failed");
            order.OrderTransactionReference = Guid.NewGuid().ToString("N")[..16];
            order.OrderStatusId = 1;
            order.OrderTax = 0.05m; 
            order.OrderTotalAmount = Math.Round(totalAmount * 1.05m, 2);

            if (order.OrderVariations.Count <= 0) throw new Exception("No variations");

            if (createOrderDto.DiscountId != null)
            {
                // await _discountService.ApplyDiscountAsync(order, createOrderDto.DiscountCode);
            }
            await _orderRepository.CreateAsync(order);   

            await _orderRepository.SaveAsync();

            return _mapper.Map<OrderDto>(await _orderRepository.GetOrderDetailsByIdAsync(order.OrderId));
        }

        public async Task CheckStockAvailabilityAsync(CreateOrderDto createOrderDto)
        {
            // Đây là logic giả định, bạn cần triển khai `IStockService`
            // foreach (var detail in createOrderDto.OrderVariationSingles)
            // {
            //     var availableStock = await _stockService.GetStockForVariation(detail.VariationSingleId);
            //     if (availableStock < detail.Quantity) // Cần thêm Quantity vào DTO
            //     {
            //         throw new InvalidOperationException($"Sản phẩm ID {detail.VariationSingleId} không đủ hàng.");
            //     }
            // }
            await Task.CompletedTask; // Xóa dòng này khi có logic thật
        }

        public async Task CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            // Chỉ cho phép hủy đơn hàng ở trạng thái 'PENDING' hoặc 'PROCESSING'
            if (order.OrderStatusId == 1 || order.OrderStatusId == 2)
            {
                order.OrderStatusId = 5; // Giả định ID 5 là "CANCELLED"
                _orderRepository.Update(order);

                // Hoàn lại hàng vào kho
                // await _stockService.ReturnStockFromOrderAsync(order);

                await _orderRepository.SaveAsync();

                // Gửi email thông báo hủy
                // await _emailService.SendOrderCancellationEmailAsync(order);
            }
            else
            {
                throw new InvalidOperationException("Không thể hủy đơn hàng ở trạng thái này.");
            }
        }

        public async Task ReviewOrderByStaffAsync(Guid orderId, Guid staffId, int orderStatus)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            if (order.OrderStatusId != 1)
            {
                throw new InvalidOperationException("Chỉ có thể duyệt đơn hàng đang chờ xử lý.");
            }
            if(orderStatus == 2)
            {
                try
                {
                    await _inventoryApiClient.DecreaseStocksAsync([.. order.OrderVariations]);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            order.InchargeEmployeeId = staffId;
            order.OrderStatusId = orderStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
        }

        public async Task ConfirmOrderReceivedAsync(Guid orderId)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            if (order.OrderStatusId != 3)
            {
                throw new InvalidOperationException("Chỉ có thể xác nhận đơn hàng đang được giao.");
            }

            order.OrderStatusId = 4;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();

            // Logic cộng điểm/tăng hạng cho khách hàng
            // await _rankService.AddMoneySpentAsync(order.UserId, order.OrderTotalAmount);
        }

        // ... triển khai các phương thức Return, Inspect, CompleteReturn tương tự

        private string GenerateTrackingNumber()
        {
            const string prefix = "TN";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomPart = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return prefix + randomPart;
        }

        public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(string? search, string? sort, int? statusId, int page, int size)
        {
            var pagedResult = await _orderRepository.GetOrdersAsync(search, sort, statusId, page, size);
            return new PagedResult<OrderDto>
            {
                Items = _mapper.Map<List<OrderDto>>(pagedResult.Items),
                TotalItems = pagedResult.TotalItems
            };
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderDetailsByIdAsync(orderId);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<PagedResult<OrderDto>> GetOrdersByCustomerIdAsync(Guid userId, int page, int size)
        {
             (var result, var total) = await _orderRepository.GetOrdersByUserIdAsync(userId, page, size);

            return new PagedResult<OrderDto>()
            {
                Items = [.. _mapper.Map<IEnumerable<OrderDto>>(result)],
                TotalItems = total
            };
        }

        // Các hàm chưa triển khai
        public Task ReturnOrderAsync(Guid orderId) => throw new NotImplementedException();
        public Task InspectReturnedOrderAsync(Guid orderId) => throw new NotImplementedException();
        public Task CompleteOrderReturnAsync(Guid orderId) => throw new NotImplementedException();
    }
}