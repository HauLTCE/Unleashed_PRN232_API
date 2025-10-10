using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories
{
    public class OrderVariationSingleRepo : IOrderVariationSingleRepo
    {
        private readonly OrderDbContext _context;
        public OrderVariationSingleRepo(OrderDbContext context) {
            _context = context;
        }
        public IQueryable<OrderVariationSingle> All()
        {
            return _context.OrderVariationSingles.AsQueryable();
        }

        public async Task<bool> CreateAsync(OrderVariationSingle entity)
        {
            try
            {
                await _context.OrderVariationSingles.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(OrderVariationSingle entity)
        {
            try 
            {
                _context.OrderVariationSingles.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<OrderVariationSingle?> FindAsync((Guid, int) id)
        {
            return await _context.OrderVariationSingles.FindAsync(id.Item1, id.Item2);
        }

        public async Task<bool> IsAny((Guid, int) id)
        {
            return await _context.OrderVariationSingles.AnyAsync(ovs => ovs.OrderId == id.Item1 && ovs.VariationSingleId == id.Item2);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Update(OrderVariationSingle entity)
        {
            try
            {
                _context.OrderVariationSingles.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
