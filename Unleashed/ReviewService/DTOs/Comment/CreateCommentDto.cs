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

        // For creating a reply to another comment
        public int? ParentCommentId { get; set; }
    }
}