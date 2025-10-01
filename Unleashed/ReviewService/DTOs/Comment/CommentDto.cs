namespace ReviewService.DTOs.Comment
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int? ReviewId { get; set; }
        public string? CommentContent { get; set; }
        public DateTimeOffset? CommentCreatedAt { get; set; }
        public DateTimeOffset? CommentUpdatedAt { get; set; }
    }
}