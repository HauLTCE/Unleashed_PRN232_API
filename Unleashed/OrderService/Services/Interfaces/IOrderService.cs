using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<ActionResult<IEnumerable<OrderDto>>> GetOrders();
        Task<ActionResult<OrderDto>> GetOrder(string id);
        Task<IActionResult> PutOrder(string id, UpdateOrderDto updateOrderDto);
        Task<ActionResult<OrderDto>> PostOrder(CreateOrderDto createOrderDto);
        Task<IActionResult> DeleteOrder(string id);
    }
}