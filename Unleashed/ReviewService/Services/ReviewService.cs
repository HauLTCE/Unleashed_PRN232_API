using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReviewService.DTOs.External;
using ReviewService.DTOs.Internal;
using ReviewService.DTOs.Review;
using ReviewService.Exceptions;
using ReviewService.Helpers;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace ReviewService.Services
{
    public class ReviewServicee : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReviewServicee(
            IReviewRepository reviewRepository,
            ICommentRepository commentRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _reviewRepository = reviewRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, Guid currentUserId)
        {
            if (reviewDto.ProductId == null || reviewDto.OrderId == null || reviewDto.UserId == null)
                throw new BadRequestException("Product, Order, and User IDs cannot be null.");

            if (currentUserId != reviewDto.UserId)
                throw new ForbiddenException("You can only create reviews for yourself.");

            if (await _reviewRepository.ExistsByProductAndOrderAndUserAsync(reviewDto.ProductId.Value, reviewDto.OrderId, reviewDto.UserId.Value))
            {
                throw new BadRequestException("You have already reviewed this product for this specific order.");
            }

            var orderClient = _httpClientFactory.CreateClient("orderservice");
            OrderDto? order;
            try
            {
                order = await orderClient.GetFromJsonAsync<OrderDto>($"api/orders/{reviewDto.OrderId}");
            }
            catch (HttpRequestException)
            {
                throw new NotFoundException("Order not found.");
            }

            if (order == null) throw new NotFoundException("Order not found.");
            if (order.UserId != currentUserId) throw new ForbiddenException("This order does not belong to you.");
            if (order.OrderStatus?.ToUpper() != "COMPLETED") throw new ForbiddenException("You can only review products from completed orders.");

            var reviewEntity = _mapper.Map<Review>(reviewDto);
            var newReview = await _reviewRepository.AddAsync(reviewEntity);

            if (!string.IsNullOrWhiteSpace(reviewDto.ReviewComment))
            {
                var comment = new Comment
                {
                    ReviewId = newReview.ReviewId,
                    CommentContent = reviewDto.ReviewComment,
                    CommentCreatedAt = DateTimeOffset.UtcNow,
                    CommentUpdatedAt = DateTimeOffset.UtcNow
                };
                await _commentRepository.AddAsync(comment);
            }

            return _mapper.Map<ReviewDto>(newReview);
        }

        public async Task<PagedResult<ProductReviewDto>> GetAllReviewsByProductIdAsync(Guid productId, int page, int size, Guid? currentUserId)
        {
            var pagedReviews = await _reviewRepository.GetTopLevelReviewsByProductIdAsync(productId, page, size);

            if (!pagedReviews.Items.Any())
            {
                return new PagedResult<ProductReviewDto>(new List<ProductReviewDto>(), 0);
            }

            var userIds = pagedReviews.Items.Select(r => r.UserId).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();
            var userClient = _httpClientFactory.CreateClient("authservice");
            var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var userMap = new Dictionary<Guid, UserDto>();
            foreach (var userId in userIds)
            {
                try
                {
                    var user = await userClient.GetFromJsonAsync<UserDto>($"api/users/{userId}", serializerOptions);
                    if (user != null) userMap[userId] = user;
                }
                catch { }
            }

            var dtos = new List<ProductReviewDto>();
            foreach (var review in pagedReviews.Items)
            {
                var rootComment = await _commentRepository.FindRootCommentByReviewIdAsync(review.ReviewId);
                var user = review.UserId.HasValue && userMap.ContainsKey(review.UserId.Value) ? userMap[review.UserId.Value] : null;

                dtos.Add(new ProductReviewDto
                {
                    ReviewId = review.ReviewId,
                    ReviewRating = review.ReviewRating,
                    ReviewComment = rootComment?.CommentContent,
                    CommentId = rootComment?.CommentId ?? 0,
                    CreatedAt = rootComment?.CommentCreatedAt ?? DateTimeOffset.MinValue,
                    FullName = user?.Username,
                    UserImage = user?.UserImage
                });
            }

            return new PagedResult<ProductReviewDto>(dtos, pagedReviews.TotalCount);
        }

        public async Task<IEnumerable<ReviewEligibilityDto>> GetReviewEligibilityAsync(Guid productId, Guid userId)
        {
            var orderClient = _httpClientFactory.CreateClient("orderservice");
            var eligibleOrders = await orderClient.GetFromJsonAsync<List<OrderDto>>($"api/orders/user/{userId}/eligible-for-review?productId={productId}");

            if (eligibleOrders == null || !eligibleOrders.Any())
            {
                return new List<ReviewEligibilityDto>();
            }

            var results = new List<ReviewEligibilityDto>();
            foreach (var order in eligibleOrders)
            {
                bool exists = await _reviewRepository.ExistsByProductAndOrderAndUserAsync(productId, order.OrderId, userId);
                if (!exists)
                {
                    results.Add(new ReviewEligibilityDto { OrderId = order.OrderId, OrderDate = order.OrderDate });
                }
            }
            return results;
        }

        public async Task<bool> CheckReviewExistsAsync(Guid productId, string orderId, Guid userId)
        {
            return await _reviewRepository.ExistsByProductAndOrderAndUserAsync(productId, orderId, userId);
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDto reviewDto)
        {
            var reviewToUpdate = await _reviewRepository.GetByIdAsync(id);
            if (reviewToUpdate == null) return false;

            _mapper.Map(reviewDto, reviewToUpdate);

            try
            {
                await _reviewRepository.UpdateAsync(reviewToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _reviewRepository.ExistsAsync(id)) return false;
                else throw;
            }
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            if (!await _reviewRepository.ExistsAsync(id)) return false;
            await _reviewRepository.DeleteAsync(id);
            return true;
        }

        public Task<PagedResult<ProductReviewDto>> GetRepliesForCommentAsync(int commentId, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<UserReviewHistoryDto>> GetReviewsByUserIdAsync(Guid userId, int page, int size)
        {
            throw new NotImplementedException();
        }
    }
}