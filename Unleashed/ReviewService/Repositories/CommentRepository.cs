using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ReviewDbContext _context;

        public CommentRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Comments.AnyAsync(e => e.CommentId == id);
        }

        public async Task AddCommentParentLinkAsync(int childId, int parentId)
        {
            var commentParent = new CommentParent
            {
                CommentId = childId,
                CommentParentId = parentId
            };
            _context.CommentParents.Add(commentParent);
            await _context.SaveChangesAsync();
        }
    }
}