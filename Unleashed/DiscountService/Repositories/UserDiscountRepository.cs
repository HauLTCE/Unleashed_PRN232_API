using DiscountService.Models;
using DiscountService.Repositories.Interfaces;

namespace DiscountService.Repositories
{
    public class UserDiscountRepository : IUserDiscountRepository
    {
        public IQueryable<UserDiscount> All()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(UserDiscount entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(UserDiscount entity)
        {
            throw new NotImplementedException();
        }

        public Task<UserDiscount?> FindAsync((Guid, int) id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAny((Guid, int) id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAsync()
        {
            throw new NotImplementedException();
        }

        public bool Update(UserDiscount entity)
        {
            throw new NotImplementedException();
        }
    }
}
