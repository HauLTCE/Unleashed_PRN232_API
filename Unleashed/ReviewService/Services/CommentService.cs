using AutoMapper;
using ReviewService.DTOs.Comment;
using ReviewService.DTOs.External;
using ReviewService.Exceptions;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReviewService.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TimeZoneInfo _targetTimeZone;

        public CommentService(
            ICommentRepository commentRepository,
            IReviewRepository reviewRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _commentRepository = commentRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
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

            if (!parentComment.ReviewId.HasValue)
                throw new InvalidOperationException("Parent comment is not linked to a review.");

            var review = await _reviewRepository.GetByIdAsync(parentComment.ReviewId.Value);
            if (review == null) throw new NotFoundException("Associated review not found.");

            var currentTime = GetCurrentTimeInTargetZone();
            var newCommentEntity = new Comment
            {
                ReviewId = parentComment.ReviewId,
                CommentContent = commentDto.CommentContent,
                CommentCreatedAt = currentTime,
                CommentUpdatedAt = currentTime
            };
            var savedComment = await _commentRepository.AddAsync(newCommentEntity);

            await _commentRepository.AddCommentParentLinkAsync(savedComment.CommentId, parentComment.CommentId);

            _ = Task.Run(async () => {
                var authClient = _httpClientFactory.CreateClient("authservice");
                var notificationClient = _httpClientFactory.CreateClient("notificationservice");
                var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    var parentAuthor = await authClient.GetFromJsonAsync<UserDto>($"api/users/{review.UserId}", serializerOptions);
                    var replyingUser = await authClient.GetFromJsonAsync<UserDto>($"api/users/{replyingUserId}", serializerOptions);

                    if (parentAuthor != null && replyingUser != null && parentAuthor.Id != replyingUser.Id)
                    {
                        var notification = new NotificationRequestDto
                        {
                            NotificationTitle = "New Reply To Your Comment",
                            NotificationContent = $"{replyingUser.Username} replied to your comment.",
                            UserIds = new List<string> { parentAuthor.Id.ToString() }
                        };
                        await notificationClient.PostAsJsonAsync("api/notifications", notification);
                    }
                }
                catch { }
            });

            return _mapper.Map<CommentDto>(savedComment);
        }

        public async Task<bool> UpdateCommentAsync(int id, UpdateCommentDto commentDto, Guid currentUserId)
        {
            var commentToUpdate = await _commentRepository.GetByIdAsync(id);
            if (commentToUpdate == null) return false;

            _mapper.Map(commentDto, commentToUpdate);
            commentToUpdate.CommentUpdatedAt = GetCurrentTimeInTargetZone();

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
            if (commentToDelete == null) return false;

            var review = await _reviewRepository.GetByIdAsync(commentToDelete.ReviewId.GetValueOrDefault());

            bool isAuthor = review?.UserId == currentUserId;
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