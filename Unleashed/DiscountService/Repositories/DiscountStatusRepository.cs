using DiscountService.Data;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DiscountService.Repositories
{
    public class DiscountStatusRepository : IDiscountStatusRepository
    {
        private readonly DiscountDbContext _dbContext;
        public DiscountStatusRepository(DiscountDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<DiscountStatus> All()
        {
            return _dbContext.DiscountStatuses.AsQueryable();
        }

        public async Task<bool> CreateAsync(DiscountStatus entity)
        {
            try
            {
                await _dbContext.DiscountStatuses.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(DiscountStatus entity)
        {
            _dbContext.DiscountStatuses.Remove(entity);
            return true;
        }

        public async Task<DiscountStatus?> FindAsync(int id)
        {
            return await _dbContext.DiscountStatuses.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _dbContext.DiscountStatuses.AnyAsync(d => d.DiscountStatusId == id);
        }

        public async Task<bool> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public bool Update(DiscountStatus entity)
        {
            try
            {
                _dbContext.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
