namespace ReviewService.DTOs.Internal
{
    public class ProductRatingSummaryDto
    {
        public Guid ProductId { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
