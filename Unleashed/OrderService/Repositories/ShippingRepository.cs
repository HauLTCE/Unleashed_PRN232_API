using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories
{
    public class ShippingRepository : IShippingRepo
    {
        private readonly OrderDbContext _context;
        public ShippingRepository(OrderDbContext context)
        {
            _context = context;
        }
        public IQueryable<ShippingMethod> All()
        {
            return _context.ShippingMethods.AsQueryable();
        }

        public async Task<bool> CreateAsync(ShippingMethod entity)
        {
            try 
            {
                await _context.ShippingMethods.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(ShippingMethod entity)
        {
            try 
            {
                _context.ShippingMethods.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ShippingMethod?> FindAsync(int id)
        {
            return await _context.ShippingMethods.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _context.ShippingMethods.AnyAsync(sm => sm.ShippingMethodId == id);
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

        public bool Update(ShippingMethod entity)
        {
            try 
            {
                _context.ShippingMethods.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
