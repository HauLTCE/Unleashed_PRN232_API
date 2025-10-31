namespace ReviewService.DTOs.Internal
{
    public class ReviewEligibilityDto
    {
        public Guid OrderId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
    }
}
