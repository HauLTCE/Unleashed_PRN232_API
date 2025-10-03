using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Guid, Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(int statusId);
    }
}