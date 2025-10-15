using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
namespace OrderService.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResult<OrderDto>> GetAllOrdersAsync(string? search, string? sort, int? statusId, int page, int size);
        Task<OrderDto?> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId);

        // Logic nghiệp vụ mới
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task CheckStockAvailabilityAsync(CreateOrderDto createOrderDto);
        Task CancelOrderAsync(Guid orderId);
        Task ReviewOrderByStaffAsync(Guid orderId, Guid staffId, bool isApproved);
        Task ConfirmOrderReceivedAsync(Guid orderId);
        Task ReturnOrderAsync(Guid orderId);
        Task InspectReturnedOrderAsync(Guid orderId);
        Task CompleteOrderReturnAsync(Guid orderId);
    }
}