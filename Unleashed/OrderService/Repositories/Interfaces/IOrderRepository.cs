using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Guid, Order>
    {
        Task<PagedResult<Order>> GetOrdersAsync(string? search, string? sort, int? statusId, int page, int size);

        Task<Order?> GetOrderDetailsByIdAsync(Guid orderId);
        Task<(IEnumerable<Order>, int total)> GetOrdersByUserIdAsync(Guid userId, int page, int size);
    }
}