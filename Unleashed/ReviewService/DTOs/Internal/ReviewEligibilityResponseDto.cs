namespace ReviewService.DTOs.Internal
{
    public class ReviewEligibilityResponseDto
    {
        public bool IsEligible { get; set; }
        public Guid? EligibleOrderId { get; set; }
        public string Message { get; set; }
    }
}
