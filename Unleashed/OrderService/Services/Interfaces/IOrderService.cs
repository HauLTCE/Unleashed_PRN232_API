using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
namespace OrderService.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(int statusId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto?> UpdateOrderAsync(Guid orderId, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(Guid orderId);
    }
}