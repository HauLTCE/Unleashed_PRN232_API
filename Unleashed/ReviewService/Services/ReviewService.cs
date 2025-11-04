using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReviewService.Clients;
using ReviewService.Clients.Interfaces;
using ReviewService.DTOs.External;
using ReviewService.DTOs.Internal;
using ReviewService.DTOs.Review;
using ReviewService.Exceptions;
using ReviewService.Helpers;
using ReviewService.Models;
using ReviewService.Repositories.Interfaces;
using ReviewService.Services.Interfaces;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ReviewService.Services
{
    public class ReviewServicee : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IProductServiceClient _productServiceClient;
        private readonly IOrderServiceClient _orderServiceClient;
        private readonly ILogger<ReviewServicee> _logger;

        public ReviewServicee(
            IReviewRepository reviewRepository,
            ICommentRepository commentRepository,
            IMapper mapper,
            IAuthServiceClient authServiceClient,
            IProductServiceClient productServiceClient,
            IOrderServiceClient orderServiceClient,
            ILogger<ReviewServicee> logger)
        {
            _reviewRepository = reviewRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _productServiceClient = productServiceClient;
            _orderServiceClient = orderServiceClient;
            _logger = logger;
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, Guid currentUserId)
        {
            if (reviewDto.ProductId == null || reviewDto.UserId == null)
                throw new BadRequestException("Product and User IDs cannot be null.");

            if (currentUserId != reviewDto.UserId)
                throw new ForbiddenException("You can only create reviews for yourself.");

            if (await _reviewRepository.ExistsByProductAndOrderAndUserAsync(reviewDto.ProductId.Value, reviewDto.OrderId, reviewDto.UserId.Value))
            {
                throw new BadRequestException("You have already reviewed this product for this specific order.");
            }

            var order = await _orderServiceClient.GetOrderByIdAsync(reviewDto.OrderId);

            if (order == null) throw new NotFoundException("Order not found or OrderService is unavailable.");

            if (order.UserId != currentUserId) throw new ForbiddenException("This order does not belong to you.");

            if (order.OrderStatusId != 4) throw new ForbiddenException("You can only review products from completed orders.");

            //_logger.LogCritical(order.OrderStatus.ToString()); //BRUH ORDER STATUS IS NULL BRO THE ORDER THING IS NOT WORKING WHY EVEN GET ORDER?

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

            var userIds = pagedReviews.Items.Select(r => r.UserId).Where(id => id.HasValue).Select(id => id.Value).Distinct();

            var employeesMap = new Dictionary<Guid, UserDto>();
            try
            {
                var employeeDetails = await _authServiceClient.GetUsersByIdsAsync(userIds);
                if (employeeDetails != null)
                {
                    employeesMap = employeeDetails.ToDictionary(e => e.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user details from AuthService for reviews. Partial data will be shown.");
            }

            var dtos = new List<ProductReviewDto>();
            foreach (var review in pagedReviews.Items)
            {
                var rootComment = await _commentRepository.FindRootCommentByReviewIdAsync(review.ReviewId);
                employeesMap.TryGetValue(review.UserId ?? Guid.Empty, out var user);

                dtos.Add(new ProductReviewDto
                {
                    ReviewId = review.ReviewId,
                    ReviewRating = review.ReviewRating,
                    ReviewComment = rootComment?.CommentContent,
                    CommentId = rootComment?.CommentId ?? 0,
                    CreatedAt = rootComment?.CommentCreatedAt ?? DateTimeOffset.MinValue,
                    UpdatedAt = rootComment?.CommentUpdatedAt ?? DateTimeOffset.MinValue,
                    FullName = user?.Username,
                    UserImage = user?.UserImage
                });
            }

            if (currentUserId.HasValue && page == 0 && dtos.Any())
            {
                var currentUserDto = employeesMap.GetValueOrDefault(currentUserId.Value);
                if (currentUserDto != null)
                {
                    var currentUserReviewIndex = dtos.FindIndex(d => d.FullName == currentUserDto.Username);
                    if (currentUserReviewIndex > 0)
                    {
                        var myReview = dtos[currentUserReviewIndex];
                        dtos.RemoveAt(currentUserReviewIndex);
                        dtos.Insert(0, myReview);
                    }
                }
            }

            return new PagedResult<ProductReviewDto>(dtos, pagedReviews.TotalCount);
        }

        public async Task<bool> GetReviewEligibilityAsync(Guid productId, Guid userId)
        {
            List<OrderDto>? eligibleOrders;
            try
            {
                eligibleOrders = await _orderServiceClient.GetEligibleOrdersForReviewAsync(userId, productId);
            }
            catch (HttpRequestException)
            {
                _logger.LogWarning("Could not check review eligibility. OrderService may be down.");
                throw new NotFoundException("Could not check review eligibility.");
            }

            if (eligibleOrders == null || !eligibleOrders.Any())
            {
                throw new BadRequestException("No eligibility order found.");
            }

            foreach (var order in eligibleOrders)
            {
                bool exists = await _reviewRepository.ExistsByProductAndOrderAndUserAsync(productId, order.OrderId, userId);
                if (!exists)
                {
                   return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckReviewExistsAsync(Guid productId, Guid orderId, Guid userId)
        {
            return await _reviewRepository.ExistsByProductAndOrderAndUserAsync(productId, orderId, userId);
        }

        public async Task<PagedResult<ProductReviewDto>> GetRepliesForCommentAsync(int commentId, int page, int size)
        {
            var pagedComments = await _reviewRepository.GetChildCommentsPaginatedAsync(commentId, page, size);

            if (!pagedComments.Items.Any())
            {
                return new PagedResult<ProductReviewDto>(new List<ProductReviewDto>(), 0);
            }

            var userIds = pagedComments.Items
                .Where(c => c.Review != null && c.Review.UserId.HasValue)
                .Select(c => c.Review.UserId.Value)
                .Distinct();

            var employeesMap = new Dictionary<Guid, UserDto>();
            try
            {
                var employeeDetails = await _authServiceClient.GetUsersByIdsAsync(userIds);
                if (employeeDetails != null)
                {
                    employeesMap = employeeDetails.ToDictionary(e => e.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user details from AuthService for replies. Partial data will be shown.");
            }

            var dtos = pagedComments.Items.Select(comment =>
            {
                UserDto? user = null;
                if (comment.Review != null && comment.Review.UserId.HasValue)
                {
                    employeesMap.TryGetValue(comment.Review.UserId.Value, out user);
                }

                return new ProductReviewDto
                {
                    ReviewId = comment.ReviewId ?? 0,
                    CommentId = comment.CommentId,
                    ReviewComment = comment.CommentContent,
                    CreatedAt = comment.CommentCreatedAt ?? DateTimeOffset.MinValue,
                    UpdatedAt = comment.CommentUpdatedAt ?? DateTimeOffset.MinValue,
                    FullName = user?.Username,
                    UserImage = user?.UserImage
                };
            }).ToList();

            return new PagedResult<ProductReviewDto>(dtos, pagedComments.TotalCount);
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
            await _reviewRepository.UpdateAsync(reviewToUpdate);
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            if (!await _reviewRepository.ExistsAsync(id)) return false;
            await _reviewRepository.DeleteAsync(id);
            return true;
        }

        public async Task<PagedResult<UserReviewHistoryDto>> GetReviewsByUserIdAsync(Guid userId, int page, int size)
        {

            var pagedReviews = await _reviewRepository.GetReviewsByUserIdAsync(userId, page, size);
            if (!pagedReviews.Items.Any())
            {
                return new PagedResult<UserReviewHistoryDto>(new List<UserReviewHistoryDto>(), 0);
            }


            var productIds = pagedReviews.Items.Where(r => r.ProductId.HasValue).Select(r => r.ProductId.Value).Distinct();

            var productDetails = await _productServiceClient.GetProductsByIdsAsync(productIds);
            var productDetailsMap = productDetails.ToDictionary(p => p.ProductId);

            var dtos = new List<UserReviewHistoryDto>();
            foreach (var review in pagedReviews.Items)
            {
                var rootComment = await _commentRepository.FindRootCommentByReviewIdAsync(review.ReviewId);
                productDetailsMap.TryGetValue(review.ProductId ?? Guid.Empty, out var product);

                dtos.Add(new UserReviewHistoryDto
                {
                    Id = review.ReviewId,
                    ReviewRating = review.ReviewRating,
                    ProductId = review.ProductId,
                    ProductName = product?.ProductName,
                    ProductImageUrl = product?.ProductImageUrl,
                    CommentContent = rootComment?.CommentContent,
                    CommentCreatedAt = rootComment?.CommentCreatedAt
                });
            }

            return new PagedResult<UserReviewHistoryDto>(dtos, pagedReviews.TotalCount);
        }

        public async Task<PagedResult<DashboardReviewDto>> GetDashboardReviewsAsync(int page, int size)
        {
            var pagedReviews = await _reviewRepository.GetRecentReviewsAsync(page, size);
            if (!pagedReviews.Items.Any())
            {
                return new PagedResult<DashboardReviewDto>(new List<DashboardReviewDto>(), 0);
            }

            var userIds = pagedReviews.Items.Where(r => r.UserId.HasValue).Select(r => r.UserId.Value).Distinct();
            var productIds = pagedReviews.Items.Where(r => r.ProductId.HasValue).Select(r => r.ProductId.Value).Distinct();

            var usersTask = _authServiceClient.GetUsersByIdsAsync(userIds);
            var productsTask = _productServiceClient.GetProductsByIdsAsync(productIds);

            await Task.WhenAll(usersTask, productsTask);

            var usersMap = usersTask.Result.ToDictionary(u => u.Id);
            var productsMap = productsTask.Result.ToDictionary(p => p.ProductId);

            var dtos = new List<DashboardReviewDto>();
            foreach (var review in pagedReviews.Items)
            {
                var rootComment = review.Comments.OrderBy(c => c.CommentId).FirstOrDefault();

                usersMap.TryGetValue(review.UserId ?? Guid.Empty, out var user);
                productsMap.TryGetValue(review.ProductId ?? Guid.Empty, out var product);

                dtos.Add(new DashboardReviewDto
                {
                    ReviewId = review.ReviewId,
                    CommentId = rootComment?.CommentId ?? 0,
                    ProductId = review.ProductId ?? Guid.Empty,
                    UserFullname = user?.Username,
                    UserImage = user?.UserImage,
                    CommentCreatedAt = rootComment?.CommentCreatedAt ?? DateTimeOffset.MinValue,
                    CommentContent = rootComment?.CommentContent,
                    ReviewRating = review.ReviewRating,
                    ProductName = product?.ProductName,
                    VariationImage = product?.ProductImageUrl,
                    ParentCommentContent = null,
                    IsMaxReply = false
                });
            }

            return new PagedResult<DashboardReviewDto>(dtos, pagedReviews.TotalCount);
        }
    }
}