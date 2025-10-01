using ReviewService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddCommentParentLinkAsync(int childId, int parentId);
    }
}