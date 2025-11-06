using System.ComponentModel.DataAnnotations;

namespace ReviewService.DTOs.Review
{
    public class UpdateReviewDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? ReviewRating { get; set; }
        [StringLength(500)]
        public string? ReviewComment { get; set; }
    }
}