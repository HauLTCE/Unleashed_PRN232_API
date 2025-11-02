namespace ReviewService.DTOs.Internal
{
    public class UserReviewHistoryDto
    {
        public int Id { get; set; }
        public int? ReviewRating { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public string? CommentContent { get; set; }
        public DateTimeOffset? CommentCreatedAt { get; set; }
    }
}
