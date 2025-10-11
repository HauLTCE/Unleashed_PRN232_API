namespace ReviewService.DTOs.Internal
{
    public class ProductReviewDto
    {
        public int ReviewId { get; set; }
        public int CommentId { get; set; }
        public string? FullName { get; set; } // From AuthService
        public string? UserImage { get; set; } // From AuthService
        public int? ReviewRating { get; set; }
        public string? ReviewComment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<ProductReviewDto> ChildComments { get; set; } = new();
    }
}
