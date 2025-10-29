// Repositories/UserDiscountRepository.cs
using DiscountService.Data;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Repositories
{
    public class UserDiscountRepository : IUserDiscountRepository
    {
        private readonly DiscountDbContext _context;

        public UserDiscountRepository(DiscountDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<UserDiscount> userDiscounts)
        {
            await _context.UserDiscounts.AddRangeAsync(userDiscounts);
        }

        public IQueryable<UserDiscount> All()
        {
            return _context.UserDiscounts.AsQueryable();
        }

        public async Task<bool> CreateAsync(UserDiscount entity)
        {
            await _context.UserDiscounts.AddAsync(entity);
            return true;
        }

        public bool Delete(UserDiscount entity)
        {
            _context.UserDiscounts.Remove(entity);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid userId, int discountId)
        {
            return await _context.UserDiscounts.AnyAsync(ud => ud.UserId == userId && ud.DiscountId == discountId);
        }

        // Chỉnh sửa lại khóa chính từ (Guid, int) thành (int, Guid) cho nhất quán
        public async Task<UserDiscount?> FindAsync((Guid,int) id)
        {
            return await _context.UserDiscounts.FindAsync(id.Item1, id.Item2);
        }

        public async Task<List<int>> FindDiscountIdsByUserIdAsync(Guid userId)
        {
            return await _context.UserDiscounts
                .Where(ud => ud.UserId == userId)
                .Select(ud => ud.DiscountId)
                .ToListAsync();
        }

        public async Task<UserDiscount?> FindByUserIdAndDiscountIdAsync(Guid userId, int discountId)
        {
            return await _context.UserDiscounts.FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DiscountId == discountId);
        }

        public async Task<List<UserDiscount>> FindByUserIdAsync(Guid userId)
        {
            return await _context.UserDiscounts.Where(ud => ud.UserId == userId).ToListAsync();
        }

        public async Task<bool> IsAny((Guid,int) id)
        {
            return await _context.UserDiscounts.AnyAsync(ud => ud.UserId == id.Item1 && ud.DiscountId == id.Item2);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Update(UserDiscount entity)
        {
            _context.UserDiscounts.Update(entity);
            return true;
        }
    }
}