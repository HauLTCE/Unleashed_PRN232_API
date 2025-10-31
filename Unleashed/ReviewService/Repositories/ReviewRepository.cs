using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Helpers;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;

        public ReviewRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                                 .Include(r => r.Comments)
                                 .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                                 .Include(r => r.Comments)
                                 .FirstOrDefaultAsync(r => r.ReviewId == id);
        }

        public async Task<Review> AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reviews.AnyAsync(e => e.ReviewId == id);
        }

        public async Task<bool> ExistsByProductAndOrderAndUserAsync(Guid productId, Guid orderId, Guid userId)
        {
            return await _context.Reviews.AnyAsync(r =>
                r.ProductId == productId &&
                r.OrderId == orderId &&
                r.UserId == userId);
        }

        public async Task<PagedResult<Review>> GetTopLevelReviewsByProductIdAsync(Guid productId, int page, int size)
        {
            var query = _context.Reviews
                .Where(r => r.ProductId == productId && r.ReviewRating != null)
                .OrderByDescending(r => r.ReviewId);

            var totalCount = await query.CountAsync();
            var items = await query.Skip(page * size).Take(size).ToListAsync();

            return new PagedResult<Review>(items, totalCount);
        }

        public async Task<PagedResult<Comment>> GetChildCommentsPaginatedAsync(int commentId, int page, int size)
        {
            var query = from c in _context.Comments
                        join cp in _context.CommentParents on c.CommentId equals cp.CommentId
                        where cp.CommentParentId == commentId
                        orderby c.CommentCreatedAt ascending
                        select c;

            var totalCount = await query.CountAsync();
            var items = await query.Skip(page * size).Take(size).ToListAsync();

            return new PagedResult<Comment>(items, totalCount);
        }

        public async Task<PagedResult<Review>> GetReviewsByUserIdAsync(Guid userId, int page, int size)
        {
            var query = _context.Reviews
                .Where(r => r.UserId == userId && r.ReviewRating != null)
                .OrderByDescending(r => r.ReviewId);

            var totalCount = await query.CountAsync();
            var items = await query.Skip(page * size).Take(size).ToListAsync();

            return new PagedResult<Review>(items, totalCount);
        }

        public async Task<PagedResult<Review>> GetRecentReviewsAsync(int page, int size)
        {
            var query = _context.Reviews
                .Include(r => r.Comments)
                .Where(r => r.ReviewRating != null)
                .OrderByDescending(r => r.ReviewId);

            var totalCount = await query.CountAsync();
            var items = await query.Skip(page * size).Take(size).ToListAsync();

            return new PagedResult<Review>(items, totalCount);
        }
    }
}