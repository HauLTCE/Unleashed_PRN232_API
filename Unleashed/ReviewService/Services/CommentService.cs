using AutoMapper;
using ReviewService.Clients.Interfaces;
using ReviewService.DTOs.Comment;
using ReviewService.DTOs.External;
using ReviewService.Exceptions;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ReviewService.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _targetTimeZone;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IProductServiceClient _productServiceClient;
        private readonly INotificationServiceClient _notificationServiceClient;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            ICommentRepository commentRepository,
            IReviewRepository reviewRepository,
            IMapper mapper,
            IAuthServiceClient authServiceClient,
            IProductServiceClient productServiceClient,
            INotificationServiceClient notificationServiceClient,
            ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _productServiceClient = productServiceClient;
            _notificationServiceClient = notificationServiceClient;
            _logger = logger;
            _targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }

        private DateTimeOffset GetCurrentTimeInTargetZone()
        {
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _targetTimeZone);
        }

        public async Task<CommentDto> CreateReplyAsync(CreateCommentDto commentDto, Guid replyingUserId)
        {
            if (!commentDto.ParentCommentId.HasValue || commentDto.ParentCommentId <= 0)
                throw new BadRequestException("A parent comment ID is required to create a reply.");

            var parentComment = await _commentRepository.GetByIdAsync(commentDto.ParentCommentId.Value);
            if (parentComment == null)
                throw new NotFoundException("Parent comment not found.");

            var review = await _reviewRepository.GetByIdAsync(parentComment.ReviewId.GetValueOrDefault());
            if (review == null || !review.ProductId.HasValue)
                throw new NotFoundException("Associated review or product not found.");

            var currentTime = DateTimeOffset.UtcNow; // Sử dụng UtcNow cho nhất quán
            var newCommentEntity = new Comment
            {
                ReviewId = parentComment.ReviewId,
                CommentContent = commentDto.CommentContent,
                CommentCreatedAt = currentTime,
                CommentUpdatedAt = currentTime,
                UserId = replyingUserId
            };

            // SỬ DỤNG PHƯƠNG THỨC REPO MỚI
            var savedComment = await _commentRepository.AddReplyAsync(newCommentEntity, parentComment.CommentId);
            await SendReplyNotificationAsync(review, replyingUserId);
            // Map kết quả
            var resultDto = _mapper.Map<CommentDto>(savedComment);
            resultDto.ParentCommentId = parentComment.CommentId; // Gán ParentId
            resultDto.userId = replyingUserId;
            return resultDto;
        }

        private async Task SendReplyNotificationAsync(Review review, Guid replyingUserId)
        {
            try
            {
                if (review.UserId == null || review.ProductId == null) return;

                if (review.UserId.Value == replyingUserId) return;

                var reviewAuthorTask = _authServiceClient.GetUsersByIdsAsync(new[] { review.UserId.Value });
                var replyingUserTask = _authServiceClient.GetUsersByIdsAsync(new[] { replyingUserId });
                var productTask = _productServiceClient.GetProductsByIdsAsync(new[] { review.ProductId.Value });

                await Task.WhenAll(reviewAuthorTask, replyingUserTask, productTask);

                var reviewAuthor = reviewAuthorTask.Result.FirstOrDefault();
                var replyingUser = replyingUserTask.Result.FirstOrDefault();
                var product = productTask.Result.FirstOrDefault();

                if (reviewAuthor != null && replyingUser != null && product != null)
                {
                    var notification = new CreateNotificationForUsersRequestDto
                    {
                        NotificationTitle = "New Reply To Your Comment",
                        NotificationContent = $"{replyingUser.UserUsername} has replied to your comment on {product.ProductName}.",
                        Usernames = new List<string> { reviewAuthor.UserUsername }
                    };

                    await _notificationServiceClient.CreateNotificationForUsersAsync(notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reply notification for review {ReviewId}", review.ReviewId);
            }
        }

        public async Task<bool> UpdateCommentAsync(int id, UpdateCommentDto commentDto, Guid currentUserId)
        {
            var commentToUpdate = await _commentRepository.GetByIdAsync(id);
            if (commentToUpdate == null)
            {
 
                return false;
            }
            if (commentToUpdate.UserId != currentUserId)
            {
                throw new ForbiddenException("You are not authorized to update this comment.");
            }
            _mapper.Map(commentDto, commentToUpdate);
            commentToUpdate.CommentUpdatedAt = DateTimeOffset.UtcNow;
            await _commentRepository.UpdateAsync(commentToUpdate);

            return true;
        }

        public async Task<CommentDto> GetCommentParentAsync(int commentId)
        {
            if (!await _commentRepository.ExistsAsync(commentId))
            {
                throw new NotFoundException("Comment not found.");
            }
            var parent = await _commentRepository.GetParentByCommentIdAsync(commentId);
            if (parent == null)
            {
                throw new NotFoundException("This comment does not have a parent.");
            }
            return _mapper.Map<CommentDto>(parent);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentDescendantsAsync(int commentId)
        {
            var descendants = (await _commentRepository.GetDescendantsAsync(commentId)).ToList();

            if (!descendants.Any())
            {
                return Enumerable.Empty<CommentDto>();
            }

            var descendantIds = descendants.Select(d => d.CommentId);

            var parentLinks = await _commentRepository.GetParentIdsForCommentsAsync(descendantIds);

            var descendantDtos = _mapper.Map<List<CommentDto>>(descendants);

            foreach (var dto in descendantDtos)
            {
                if (parentLinks.TryGetValue(dto.CommentId, out var parentId))
                {
                    dto.ParentCommentId = parentId;
                }
            }

            return descendantDtos;
        }

        public async Task<bool> DeleteCommentAsync(int id, Guid currentUserId, IEnumerable<string> roles)
        {
            var commentToDelete = await _commentRepository.GetByIdAsync(id);
            if (commentToDelete == null)
            {
                return false;
            }

            bool isAuthor = commentToDelete.UserId == currentUserId;


            bool isAdminOrStaff = roles.Contains("ADMIN") || roles.Contains("STAFF");


            if (!isAuthor && !isAdminOrStaff)
            {
                throw new ForbiddenException("You are not authorized to delete this comment.");
            }


            await RecursivelyDeleteReplies(id);


            await _commentRepository.DeleteParentLinkAsync(id);
            await _commentRepository.DeleteAsync(id);

            return true;
        }


        private async Task RecursivelyDeleteReplies(int parentId)
        {
            var replies = await _commentRepository.GetRepliesByParentIdAsync(parentId);
            foreach (var reply in replies)
            {

                await RecursivelyDeleteReplies(reply.CommentId);

                await _commentRepository.DeleteParentLinkAsync(reply.CommentId);
                await _commentRepository.DeleteAsync(reply.CommentId);
            }
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
    }
}