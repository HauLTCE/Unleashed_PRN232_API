using System.ComponentModel.DataAnnotations;

namespace ReviewService.DTOs.Review
{
    public class CreateReviewDto
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string? OrderId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? ReviewRating { get; set; }

        [StringLength(500)]
        public string? ReviewComment { get; set; }
    }
}