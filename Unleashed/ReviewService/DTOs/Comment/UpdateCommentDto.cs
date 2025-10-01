using System.ComponentModel.DataAnnotations;

namespace ReviewService.DTOs.Comment
{
    public class UpdateCommentDto
    {
        [Required]
        [StringLength(255)]
        public string? CommentContent { get; set; }
    }
}