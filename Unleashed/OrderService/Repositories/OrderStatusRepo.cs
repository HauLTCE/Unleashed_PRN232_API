using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using System.Threading.Tasks;

namespace OrderService.Repositories
{
    public class OrderStatusRepo : IOrderStatusRepo
    {
        private readonly OrderDbContext _context;
        public OrderStatusRepo(OrderDbContext context) {
            _context = context;
        }
        public IQueryable<OrderStatus> All()
        {
            return _context.OrderStatuses.AsQueryable();
        }

        public async Task<bool> CreateAsync(OrderStatus entity)
        {
            try 
            {
                await _context.OrderStatuses.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(OrderStatus entity)
        {
            try 
            {
                _context.OrderStatuses.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<OrderStatus?> FindAsync(int id)
        {
            return await _context.OrderStatuses.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _context.OrderStatuses.AnyAsync(os => os.OrderStatusId == id);
        }

        public async Task<bool> SaveAsync()
        {
            try 
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(OrderStatus entity)
        {
            try 
            {
                _context.OrderStatuses.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
