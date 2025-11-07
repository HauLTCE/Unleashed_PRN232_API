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
        private readonly IDiscountApiClient _discountApiClient;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderServices> _logger;
        public OrderServices(IOrderRepository orderRepository, IOrderStatusRepo orderStatusRepository, IOrderVariationRepo orderVariationSingleRepository, IProductApiClient productApiClient,IInventoryApiClient inventoryApiClient, IMapper mapper, ILogger<OrderServices> logger, IDiscountApiClient discountApiClient)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderVariationSingleRepository = orderVariationSingleRepository;
            _productApiClient = productApiClient;
            _inventoryApiClient = inventoryApiClient;
            _mapper = mapper;
            _logger = logger;
            _discountApiClient = discountApiClient;
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
            double subTotal = 0; // Đổi tên từ totalAmount thành subTotal để rõ ràng hơn

            foreach (var stockItem in stockLevels)
            {
                if (!requestedQty.TryGetValue(stockItem.VariationId, out var qty) || qty <= 0)
                    continue;

                if (qty > stockItem.TotalQuantity)
                    throw new InvalidOperationException(
                        $"Not enough stock for variation {stockItem.VariationId}. Requested: {qty}, Available: {stockItem.TotalQuantity}");

                if (!variationLookup.TryGetValue(stockItem.VariationId, out var variation))
                    throw new KeyNotFoundException($"Price not found for variation {stockItem.VariationId}");

                subTotal += qty!.Value * variation.VariationPrice;
            }

            var order = _mapper.Map<Order>(createOrderDto) ?? throw new Exception("Mapping failed");

            decimal discountAmount = 0m;
            if (createOrderDto.DiscountId.HasValue)
            {
                var discount = await _discountApiClient.Get(createOrderDto.DiscountId.Value);

                if (discount != null)
                {
                    // 1. Kiểm tra discount có hợp lệ không
                    if (discount.DiscountStatusId != 2) // 2: ACTIVE
                    {
                        throw new InvalidOperationException($"Discount code '{discount.DiscountCode}' is not active.");
                    }
                    if (discount.DiscountMinimumOrderValue.HasValue && Convert.ToDecimal(subTotal) < discount.DiscountMinimumOrderValue.Value)
                    {
                        throw new InvalidOperationException($"Order total does not meet the minimum requirement of {discount.DiscountMinimumOrderValue.Value} for this discount.");
                    }

                    // 2. Tính toán số tiền được giảm
                    if (discount.DiscountTypeId == 1 && discount.DiscountValue.HasValue) // 1: PERCENTAGE
                    {
                        discountAmount = Convert.ToDecimal(subTotal) * (discount.DiscountValue.Value / 100m);
                        if (discount.DiscountMaximumValue.HasValue && discountAmount > discount.DiscountMaximumValue.Value)
                        {
                            discountAmount = discount.DiscountMaximumValue.Value;
                        }
                    }
                    else if (discount.DiscountTypeId == 2 && discount.DiscountValue.HasValue) // 2: FLAT_AMOUNT
                    {
                        discountAmount = discount.DiscountValue.Value;
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Discount with ID {createOrderDto.DiscountId.Value} not found.");
                }
            }

            order.OrderId = Guid.NewGuid();
            order.OrderDate = DateTimeOffset.UtcNow;
            order.OrderTrackingNumber = GenerateTrackingNumber() ?? throw new Exception("Tracking number failed");
            order.OrderTransactionReference = Guid.NewGuid().ToString("N")[..16];
            order.OrderStatusId = 1;
            order.OrderTax = 0.05m;
            decimal subTotalAfterDiscount = Convert.ToDecimal(subTotal) - discountAmount;
            order.OrderTotalAmount = subTotalAfterDiscount + (subTotalAfterDiscount * order.OrderTax.Value);
            order.OrderTotalAmount = Math.Round(order.OrderTotalAmount.Value, 2);

            if (order.OrderVariations.Count <= 0) throw new Exception("No variations");

            
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

                if (order.OrderStatusId == 2)
                {
                    // Nếu đơn hàng đã được xử lý (đã trừ kho), thì cần hoàn trả hàng vào kho
                    try
                    {
                        var returnSuccess = await _inventoryApiClient.ReturnStocksAsync(order.OrderVariations.ToList());

                        // Nếu việc hoàn kho thất bại, đây là một lỗi nghiêm trọng
                        if (!returnSuccess)
                        {
                            _logger.LogError("CRITICAL: Không thể hoàn trả hàng cho đơn hàng bị hủy {OrderId}. " +
                                             "Hủy bỏ thao tác. Cần can thiệp thủ công.", orderId);
                            // Ném ra một Exception để ngăn việc cập nhật trạng thái đơn hàng khi kho chưa được đồng bộ
                            throw new Exception($"Không thể hoàn trả hàng cho đơn hàng {orderId}. Việc hủy đơn đã bị dừng lại.");
                        }

                        _logger.LogInformation("Hàng hóa cho đơn hàng bị hủy {OrderId} đã được hoàn trả thành công vào kho.", orderId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi nghiêm trọng khi gọi API hoàn trả hàng cho đơn hàng {OrderId}.", orderId);
                        throw; // Ném lại lỗi để dừng tiến trình
                    }
                }

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
                    if (order.DiscountId.HasValue)
                    {
                        await _discountApiClient.UseDiscount(order.DiscountId.Value);
                    }
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

        public async Task UpdateOrderStatusByStaffAsync(Guid orderId, Guid staffId, int newStatusId)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            // Chỉ cho phép nhân viên chuyển từ Processing (2) -> Shipping (3)
            if (order.OrderStatusId == 2 && newStatusId == 3)
            {
                order.OrderStatusId = newStatusId;
                order.InchargeEmployeeId = staffId; // Cập nhật nhân viên phụ trách
                                                    // Optional: Có thể thêm logic gán tracking number ở đây nếu cần
                _orderRepository.Update(order);
                await _orderRepository.SaveAsync();
            }
            else
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{order.OrderStatusId}' sang '{newStatusId}'.");
            }
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
            var orderDTOs = _mapper.Map<List<OrderDto>>(result);
            return new PagedResult<OrderDto>()
            {
                Items = orderDTOs,
                TotalItems = total
            };
        }

        public async Task<IEnumerable<OrderDto>> GetEligibleOrdersForReviewAsync(Guid userId, Guid productId)
        {
            var completedOrders = await _orderRepository.GetCompletedOrdersByUserIdAsync(userId);
            if (!completedOrders.Any())
            {
                return Enumerable.Empty<OrderDto>();
            }

            var allVariationIds = completedOrders.SelectMany(o => o.OrderVariations.Select(v => v.VariationId)).Distinct().ToList();

            if (!allVariationIds.Any())
            {
                return Enumerable.Empty<OrderDto>();
            }

            var variationDetails = await _productApiClient.GetDetailsByIdsAsync(allVariationIds);

            if (variationDetails == null || !variationDetails.Any())
            {
                _logger.LogWarning("No variation details found from ProductService for variation IDs: {VariationIds}", string.Join(", ", allVariationIds));
                return Enumerable.Empty<OrderDto>();
            }

            var variationDetailsWithProduct = variationDetails.ToDictionary(v => v.VariationId);

            var eligibleOrders = new List<Order>();

            foreach (var order in completedOrders)
            {
                bool hasProduct = order.OrderVariations.Any(ov =>
                    variationDetailsWithProduct.ContainsKey(ov.VariationId) &&
                    variationDetailsWithProduct[ov.VariationId].ProductId == productId);

                if (hasProduct)
                {
                    eligibleOrders.Add(order);
                }
            }

            return _mapper.Map<IEnumerable<OrderDto>>(eligibleOrders);
        }

        // Các hàm chưa triển khai
        public Task ReturnOrderAsync(Guid orderId) => throw new NotImplementedException();
        public Task InspectReturnedOrderAsync(Guid orderId) => throw new NotImplementedException();
        public Task CompleteOrderReturnAsync(Guid orderId) => throw new NotImplementedException();
    }
}