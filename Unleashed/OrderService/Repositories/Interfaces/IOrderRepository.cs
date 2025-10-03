using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(string id);
        Task AddAsync(Order order);
        void Update(Order order);
        void Remove(Order order);
        Task<bool> SaveChangesAsync();
        Task<bool> OrderExistsAsync(string id);
    }
}