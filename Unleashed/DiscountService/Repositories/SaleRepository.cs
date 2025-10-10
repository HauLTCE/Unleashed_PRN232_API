using DiscountService.Models;
using DiscountService.Repositories.Interfaces;

namespace DiscountService.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        public IQueryable<Sale> All()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(Sale entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Sale entity)
        {
            throw new NotImplementedException();
        }

        public Task<Sale?> FindAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAny(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAsync()
        {
            throw new NotImplementedException();
        }

        public bool Update(Sale entity)
        {
            throw new NotImplementedException();
        }
    }
}
