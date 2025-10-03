using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Repositories;

namespace OrderService.Services
{
    public class OrderServices : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderServices(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            return new OkObjectResult(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        public async Task<ActionResult<OrderDto>> GetOrder(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(_mapper.Map<OrderDto>(order));
        }

        public async Task<IActionResult> PutOrder(string id, UpdateOrderDto updateOrderDto)
        {
            var orderFromRepo = await _orderRepository.GetByIdAsync(id);
            if (orderFromRepo == null)
            {
                return new NotFoundResult();
            }

            _mapper.Map(updateOrderDto, orderFromRepo);
            orderFromRepo.OrderUpdatedAt = DateTimeOffset.UtcNow;

            _orderRepository.Update(orderFromRepo);

            try
            {
                await _orderRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _orderRepository.OrderExistsAsync(id))
                {
                    return new NotFoundResult();
                }
                else
                {
                    throw;
                }
            }

            return new NoContentResult();
        }

        public async Task<ActionResult<OrderDto>> PostOrder(CreateOrderDto createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);

            // Gán các giá trị mặc định khi tạo mới
            order.OrderId = "ORD-" + Guid.NewGuid().ToString("N").ToUpper();
            order.OrderDate = DateTimeOffset.UtcNow;
            order.OrderCreatedAt = DateTimeOffset.UtcNow;
            order.OrderUpdatedAt = DateTimeOffset.UtcNow;

            try
            {
                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await _orderRepository.OrderExistsAsync(order.OrderId))
                {
                    return new ConflictResult();
                }
                else
                {
                    throw;
                }
            }

            var createdOrder = await _orderRepository.GetByIdAsync(order.OrderId);
            var orderDto = _mapper.Map<OrderDto>(createdOrder);

            return new CreatedAtActionResult("GetOrder", "Orders", new { id = orderDto.OrderId }, orderDto);
        }

        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return new NotFoundResult();
            }

            _orderRepository.Remove(order);
            await _orderRepository.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}