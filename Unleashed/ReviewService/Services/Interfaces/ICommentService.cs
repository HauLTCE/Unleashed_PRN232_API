using ReviewService.DTOs.Comment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllCommentsAsync();
        Task<CommentDto?> GetCommentByIdAsync(int id);
        Task<CommentDto> CreateReplyAsync(CreateCommentDto commentDto, Guid replyingUserId);
        Task<bool> UpdateCommentAsync(int id, UpdateCommentDto commentDto, Guid currentUserId);
        Task<bool> DeleteCommentAsync(int id, Guid currentUserId, IEnumerable<string> roles);
        Task<CommentDto> GetCommentParentAsync(int commentId);
        Task<IEnumerable<CommentDto>> GetCommentDescendantsAsync(int commentId);
    }
}