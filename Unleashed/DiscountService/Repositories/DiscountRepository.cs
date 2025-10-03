using DiscountService.Data;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DiscountDbContext _context;
        public DiscountRepository(DiscountDbContext context) {
            _context = context;
        }
        public IQueryable<Discount> All()
        {
            return _context.Discounts.AsQueryable();
        }

        public async Task<bool> CreateAsync(Discount entity)
        {
            try {
                await _context.Discounts.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Discount entity)
        {
            try {
                _context.Discounts.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Discount?> FindAsync(int id)
        {
            return await _context.Discounts.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _context.Discounts.AnyAsync(d => d.DiscountId == id);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Update(Discount entity)
        {
            try
            {
                _context.Discounts.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
