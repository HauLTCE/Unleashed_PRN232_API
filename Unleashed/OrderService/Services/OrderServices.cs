using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Interfaces;
using System.Security.Cryptography;

namespace OrderService.Services
{
    public class OrderServices : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepo _orderStatusRepository;
        private readonly IOrderVariationRepo _orderVariationSingleRepository;
        private readonly IMapper _mapper;

        // Giả định đã inject các service khác (cần được bạn tạo ra)
        // private readonly IStockService _stockService;
        // private readonly IEmailService _emailService;
        // private readonly IPaymentService _paymentService;
        // private readonly IDiscountService _discountService;

        public OrderServices(IOrderRepository orderRepository, IOrderStatusRepo orderStatusRepository, IOrderVariationRepo orderVariationSingleRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderVariationSingleRepository = orderVariationSingleRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // 1. Kiểm tra tồn kho (quan trọng)
            await CheckStockAvailabilityAsync(createOrderDto);

            var order = _mapper.Map<Order>(createOrderDto);

            order.OrderVariations = [];

            // 2. Thiết lập các giá trị mặc định cho đơn hàng
            order.OrderId = Guid.NewGuid();
            order.OrderDate = DateTimeOffset.UtcNow;
            order.OrderTrackingNumber = GenerateTrackingNumber(); // Hàm tạo mã vận đơn
            order.OrderTransactionReference = Guid.NewGuid().ToString("N").Substring(0, 16); // Mã giao dịch tạm
            order.OrderStatusId = 1; // Giả định ID 1 là "PENDING"
            order.OrderTax = 0.05m; // 5% tax

            // 3. Xử lý giảm giá (nếu có)
            // await _discountService.ApplyDiscountAsync(order, createOrderDto.DiscountCode);

            await _orderRepository.CreateAsync(order);

           
            var groupedItems = createOrderDto.OrderVariations
            .GroupBy(dto => dto.VariationId) 
            .Select(group => new
            {
                RepresentingDto = group.First(),
                Total = group.Sum(dto => dto.VariationPriceAtPurchase)
            });

            foreach (var item in groupedItems)
            {
                var ovs = _mapper.Map<OrderVariation>(item.RepresentingDto);
                ovs.OrderId = order.OrderId;
                ovs.VariationPriceAtPurchase = item.Total; 

                await _orderVariationSingleRepository.CreateAsync(ovs);
            }

            // 5. Xử lý thanh toán
            // var paymentResult = await _paymentService.ProcessPayment(order, createOrderDto.PaymentMethodId);
            // order.OrderTransactionReference = paymentResult.TransactionId;

            // 6. Trừ sản phẩm khỏi kho
            // await _stockService.ReserveStockForOrderAsync(order);

            await _orderRepository.SaveAsync();

            // 7. Gửi email xác nhận
            // await _emailService.SendOrderConfirmationEmailAsync(order);

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

        public async Task ReviewOrderByStaffAsync(Guid orderId, Guid staffId, bool isApproved)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            if (order.OrderStatusId != 1)
            {
                throw new InvalidOperationException("Chỉ có thể duyệt đơn hàng đang chờ xử lý.");
            }

            order.InchargeEmployeeId = staffId;
            if (isApproved)
            {
                order.OrderStatusId = 2;
            }
            else
            {
                order.OrderStatusId = 7;
                // Hoàn kho
                // await _stockService.ReturnStockFromOrderAsync(order);
            }
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

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            // Nên sử dụng phương thức phân trang ở trên thay vì tạo hàm mới
            var pagedResult = await _orderRepository.GetOrdersAsync(null, null, null, 0, 100); // Lấy 100 đơn hàng gần nhất
            return _mapper.Map<IEnumerable<OrderDto>>(pagedResult.Items.Where(o => o.UserId == customerId));
        }

        // Các hàm chưa triển khai
        public Task ReturnOrderAsync(Guid orderId) => throw new NotImplementedException();
        public Task InspectReturnedOrderAsync(Guid orderId) => throw new NotImplementedException();
        public Task CompleteOrderReturnAsync(Guid orderId) => throw new NotImplementedException();
    }
}