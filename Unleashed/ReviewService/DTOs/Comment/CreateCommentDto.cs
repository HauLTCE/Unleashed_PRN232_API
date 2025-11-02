using System.ComponentModel.DataAnnotations;

namespace ReviewService.DTOs.Comment
{
    public class CreateCommentDto
    {
        [Required]
        public int? ReviewId { get; set; }

        [Required]
        [StringLength(255)]
        public string? CommentContent { get; set; }

        public int? ParentCommentId { get; set; }
    }
}