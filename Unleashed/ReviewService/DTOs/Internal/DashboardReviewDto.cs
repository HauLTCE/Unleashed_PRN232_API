namespace ReviewService.DTOs.Internal
{
    public class DashboardReviewDto
    {
        public int CommentId { get; set; }
        public int ReviewId { get; set; }
        public Guid ProductId { get; set; }
        public string? UserFullname { get; set; }
        public string? UserImage { get; set; }
        public DateTimeOffset CommentCreatedAt { get; set; }
        public string? CommentContent { get; set; }
        public int? ReviewRating { get; set; }
        public string? ProductName { get; set; }
        public string? VariationImage { get; set; }
        public string? ParentCommentContent { get; set; }
        public bool IsMaxReply { get; set; }
    }
}
