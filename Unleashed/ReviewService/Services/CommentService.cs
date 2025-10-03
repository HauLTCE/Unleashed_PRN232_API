using AutoMapper;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewService.DTOs.Comment;

namespace ReviewService.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto)
        {
            var commentEntity = _mapper.Map<Comment>(commentDto);
            commentEntity.CommentCreatedAt = DateTimeOffset.UtcNow;
            commentEntity.CommentUpdatedAt = DateTimeOffset.UtcNow;

            var newComment = await _commentRepository.AddAsync(commentEntity);

            // If a parent comment is specified, create the link
            if (commentDto.ParentCommentId.HasValue && commentDto.ParentCommentId > 0)
            {
                await _commentRepository.AddCommentParentLinkAsync(newComment.CommentId, commentDto.ParentCommentId.Value);
            }

            return _mapper.Map<CommentDto>(newComment);
        }

        public async Task<bool> UpdateCommentAsync(int id, UpdateCommentDto commentDto)
        {
            var commentToUpdate = await _commentRepository.GetByIdAsync(id);
            if (commentToUpdate == null)
            {
                return false;
            }

            _mapper.Map(commentDto, commentToUpdate);
            commentToUpdate.CommentUpdatedAt = DateTimeOffset.UtcNow;

            await _commentRepository.UpdateAsync(commentToUpdate);
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            if (!await _commentRepository.ExistsAsync(id))
            {
                return false;
            }
            await _commentRepository.DeleteAsync(id);
            return true;
        }
    }
}