using ReviewService.DTOs.Comment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllCommentsAsync();
        Task<CommentDto?> GetCommentByIdAsync(int id);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto);
        Task<bool> UpdateCommentAsync(int id, UpdateCommentDto commentDto);
        Task<bool> DeleteCommentAsync(int id);
    }
}