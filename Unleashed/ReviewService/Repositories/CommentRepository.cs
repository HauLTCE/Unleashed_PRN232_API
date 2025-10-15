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

        public async Task<IEnumerable<Comment>> GetRepliesByParentIdAsync(int parentId)
        {
            var replyIds = await _context.CommentParents
                .Where(cp => cp.CommentParentId == parentId)
                .Select(cp => cp.CommentId)
                .ToListAsync();

            return await _context.Comments
                .Where(c => replyIds.Contains(c.CommentId))
                .ToListAsync();
        }

        public async Task<int?> GetParentIdByCommentIdAsync(int commentId)
        {
            return await _context.CommentParents
                .Where(cp => cp.CommentId == commentId)
                .Select(cp => cp.CommentParentId)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteParentLinkAsync(int commentId)
        {
            var links = _context.CommentParents.Where(cp => cp.CommentId == commentId);
            _context.CommentParents.RemoveRange(links);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment?> FindRootCommentByReviewIdAsync(int reviewId)
        {
            return await _context.Comments
                .Where(c => c.ReviewId == reviewId && !_context.CommentParents.Any(cp => cp.CommentId == c.CommentId))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetDescendantsAsync(int rootCommentId)
        {
            var sql = @"
                WITH CommentTree AS (
                    SELECT c.*
                    FROM dbo.comment c
                    JOIN dbo.comment_parent cp ON c.comment_id = cp.comment_id
                    WHERE cp.comment_parent_id = {0}

                    UNION ALL

                    SELECT c.*
                    FROM dbo.comment c
                    JOIN dbo.comment_parent cp ON c.comment_id = cp.comment_id
                    JOIN CommentTree ct ON cp.comment_parent_id = ct.comment_id
                )
                SELECT *
                FROM CommentTree
                ORDER BY comment_created_at ASC;";

            return await _context.Comments.FromSqlRaw(sql, rootCommentId).ToListAsync();
        }

    }
}