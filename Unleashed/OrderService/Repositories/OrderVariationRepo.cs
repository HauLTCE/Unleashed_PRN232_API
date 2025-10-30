using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories
{
    public class OrderVariationRepo : IOrderVariationRepo
    {
        private readonly OrderDbContext _context;
        public OrderVariationRepo(OrderDbContext context) {
            _context = context;
        }
        public IQueryable<Models.OrderVariation> All()
        {
            return _context.OrderVariationSingles.AsQueryable();
        }

        public async Task<bool> CreateAsync(Models.OrderVariation entity)
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

        public bool Delete(Models.OrderVariation entity)
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

        public async Task<Models.OrderVariation?> FindAsync((Guid, int) id)
        {
            return await _context.OrderVariationSingles.FindAsync(id.Item1, id.Item2);
        }

        public async Task<bool> IsAny((Guid, int) id)
        {
            return await _context.OrderVariationSingles.AnyAsync(ovs => ovs.OrderId == id.Item1 && ovs.VariationId == id.Item2);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Update(Models.OrderVariation entity)
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
