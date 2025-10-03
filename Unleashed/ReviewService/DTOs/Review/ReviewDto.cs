using ReviewService.DTOs.Comment;

namespace ReviewService.DTOs.Review
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? UserId { get; set; }
        public string? OrderId { get; set; }
        public int? ReviewRating { get; set; }
        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}