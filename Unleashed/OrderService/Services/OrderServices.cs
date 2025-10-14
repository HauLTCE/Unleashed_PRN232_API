using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Interfaces;

namespace OrderService.Services
{
    public class OrderServices : IOrderService // checkout? create var single? reduce stock quantity?
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepo _orderStatusRepository;
        private readonly IPaymenMethodRepo _paymentMethodRepository;
        private readonly IShippingRepo _shippingRepository;
        private readonly IMapper _mapper;

        public OrderServices(IOrderRepository orderRepository, IMapper mapper, IOrderStatusRepo orderStatusRepository, IPaymenMethodRepo paymentMethodRepository, IShippingRepo _shippingRepo)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderStatusRepository = orderStatusRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _shippingRepository = _shippingRepo;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            if (createOrderDto.OrderStatusId.HasValue)
            {
                var status = await _orderStatusRepository.FindAsync(createOrderDto.OrderStatusId.Value);
                if (status == null)
                {
                    throw new ArgumentException($"Order status with ID {createOrderDto.OrderStatusId.Value} does not exist.");
                }
            }
            if (createOrderDto.PaymentMethodId.HasValue)
            {
                var paymentMethod = await _paymentMethodRepository.FindAsync(createOrderDto.PaymentMethodId.Value);
                if (paymentMethod == null)
                {
                    throw new ArgumentException($"Payment method with ID {createOrderDto.PaymentMethodId.Value} does not exist.");
                }
            }
            if (createOrderDto.ShippingMethodId.HasValue)
            {
                var shippingMethod = await _shippingRepository.FindAsync(createOrderDto.ShippingMethodId.Value);
                if (shippingMethod == null)
                {
                    throw new ArgumentException($"Shipping method with ID {createOrderDto.ShippingMethodId.Value} does not exist.");
                }
            }
            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderId = Guid.NewGuid();
            order.OrderCreatedAt = DateTimeOffset.UtcNow;
            order.OrderUpdatedAt = DateTimeOffset.UtcNow;
            var created = await _orderRepository.CreateAsync(order);
            if (!created)
            {
                throw new Exception("Failed to create order.");
            }
            var saved = await _orderRepository.SaveAsync();
            if (!saved)
            {
                throw new Exception("Failed to save changes after creating order.");
            }
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }
            var deleted = _orderRepository.Delete(order);
            if (!deleted)
            {
                throw new Exception("Failed to delete order.");
            }
            var saved = await _orderRepository.SaveAsync();
            if (!saved)
            {
                throw new Exception("Failed to save changes after deleting order.");
            }
            return true;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.All()
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.ShippingMethod)
                .Include(o => o.OrderVariationSingles)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.All()
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.ShippingMethod)
                .Include(o => o.OrderVariationSingles)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return null;
            }
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(int statusId)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(statusId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> UpdateOrderAsync(Guid orderId, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.FindAsync(orderId);
            if (order == null)
            {
                return null;
            }
            if (updateOrderDto.OrderStatusId.HasValue)
            {
                var status = await _orderStatusRepository.FindAsync(updateOrderDto.OrderStatusId.Value);
                if (status == null)
                {
                    throw new ArgumentException($"Order status with ID {updateOrderDto.OrderStatusId.Value} does not exist.");
                }
                order.OrderStatusId = updateOrderDto.OrderStatusId;
            }
            if (updateOrderDto.InchargeEmployeeId.HasValue)
            {
                order.InchargeEmployeeId = updateOrderDto.InchargeEmployeeId;
            }
            if (!string.IsNullOrEmpty(updateOrderDto.OrderTrackingNumber))
            {
                order.OrderTrackingNumber = updateOrderDto.OrderTrackingNumber;
            }
            if (updateOrderDto.OrderExpectedDeliveryDate.HasValue)
            {
                order.OrderExpectedDeliveryDate = updateOrderDto.OrderExpectedDeliveryDate;
            }
            if (!string.IsNullOrEmpty(updateOrderDto.OrderNote))
            {
                order.OrderNote = updateOrderDto.OrderNote;
            }
            order.OrderUpdatedAt = DateTimeOffset.UtcNow;
            var updated = _orderRepository.Update(order);
            if (!updated)
            {
                throw new Exception("Failed to update order.");
            }
            var saved = await _orderRepository.SaveAsync();
            if (!saved)
            {
                throw new Exception("Failed to save changes after updating order.");
            }
            return _mapper.Map<OrderDto>(order);
        }
    }
}