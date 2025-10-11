namespace ReviewService.DTOs.Internal
{
    public class UserReviewHistoryDto
    {
        public int Id { get; set; }
        public int? ReviewRating { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; } // From ProductService
        public string? ProductImageUrl { get; set; } // From ProductService
        public string? CommentContent { get; set; }
        public DateTimeOffset? CommentCreatedAt { get; set; }
    }
}
