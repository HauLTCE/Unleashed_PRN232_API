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
        public async Task<Comment> AddReplyAsync(Comment reply, int parentId)
        {
            // Transaction đảm bảo cả hai thao tác cùng thành công hoặc thất bại
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Thêm comment con (reply)
                _context.Comments.Add(reply);
                await _context.SaveChangesAsync(); // Lưu để 'reply' có được CommentId

                // 2. Tạo liên kết cha-con
                var commentParent = new CommentParent
                {
                    CommentId = reply.CommentId,
                    CommentParentId = parentId
                };
                _context.CommentParents.Add(commentParent);
                await _context.SaveChangesAsync(); // Lưu liên kết

                // 3. Nếu mọi thứ thành công, commit transaction
                await transaction.CommitAsync();

                return reply;
            }
            catch (Exception)
            {
                // 4. Nếu có lỗi, rollback tất cả thay đổi
                await transaction.RollbackAsync();
                throw; // Ném lại exception để lớp Service xử lý
            }
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
                // Logic xóa đệ quy nên được xử lý ở Service, Repository chỉ xóa 1 bản ghi
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
            //await _context.SaveChangesAsync();
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

        public async Task<Comment?> GetParentByCommentIdAsync(int commentId)
        {
            var parentId = await _context.CommentParents
                .Where(cp => cp.CommentId == commentId)
                .Select(cp => cp.CommentParentId)
                .FirstOrDefaultAsync();

            if (parentId.HasValue)
            {
                return await GetByIdAsync(parentId.Value);
            }

            return null;
        }

        public async Task DeleteParentLinkAsync(int commentId)
        {
            // Sửa lại để xóa đúng bản ghi
            var links = _context.CommentParents.Where(cp => cp.CommentId == commentId || cp.CommentParentId == commentId);
            if (links.Any())
            {
                _context.CommentParents.RemoveRange(links);
                await _context.SaveChangesAsync();
            }
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
                    SELECT cp.comment_id
                    FROM dbo.comment_parent cp
                    WHERE cp.comment_parent_id = {0}

                    UNION ALL

                    SELECT cp.comment_id
                    FROM dbo.comment_parent cp
                    INNER JOIN CommentTree ct ON cp.comment_parent_id = ct.comment_id
                )
                SELECT c.*
                FROM dbo.comment c
                WHERE c.comment_id IN (SELECT comment_id FROM CommentTree);";

            return await _context.Comments.FromSqlRaw(sql, rootCommentId).ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetParentIdsForCommentsAsync(IEnumerable<int> commentIds)
        {
            return await _context.CommentParents
                .Where(cp => cp.CommentId.HasValue && commentIds.Contains(cp.CommentId.Value) && cp.CommentParentId.HasValue)
                .ToDictionaryAsync(cp => cp.CommentId.Value, cp => cp.CommentParentId.Value);
        }

    }
}