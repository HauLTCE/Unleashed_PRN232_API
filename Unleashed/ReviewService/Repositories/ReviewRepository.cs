using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
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
    }
}