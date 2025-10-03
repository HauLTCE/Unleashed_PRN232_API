using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> All()
        {
            return _context.Orders.AsQueryable();
        }

        public async Task<bool> CreateAsync(Order entity)
        {
            try 
            {
                await _context.Orders.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Order entity)
        {
            try 
            {
                _context.Orders.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Order?> FindAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders
                .Where(o => o.UserId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(int statusId)
        {
            return await _context.Orders
                .Where(o => o.OrderStatusId == statusId)
                .ToListAsync();
        }

        public async Task<bool> IsAny(Guid id)
        {
            return await _context.Orders.AnyAsync(e => e.OrderId == id);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Update(Order entity)
        {
            try 
            {
                _context.Orders.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}