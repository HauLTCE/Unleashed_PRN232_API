using DiscountService.Data;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DiscountService.Repositories
{
    public class DiscountTypeRepository : IDiscountTypeRepository
    {
        private readonly DiscountDbContext _dbContext;
        public DiscountTypeRepository(DiscountDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<DiscountType> All()
        {
            return _dbContext.DiscountTypes.AsQueryable();
        }

        public async Task<bool> CreateAsync(DiscountType entity)
        {
            try
            {
                await _dbContext.DiscountTypes.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(DiscountType entity)
        {
            _dbContext.DiscountTypes.Remove(entity);
            return true;
        }

        public async Task<DiscountType?> FindAsync(int id)
        {
            return await _dbContext.DiscountTypes.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _dbContext.DiscountTypes.AnyAsync(d => d.DiscountTypeId == id);
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public bool Update(DiscountType entity)
        {
            try
            {
                _dbContext.DiscountTypes.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
