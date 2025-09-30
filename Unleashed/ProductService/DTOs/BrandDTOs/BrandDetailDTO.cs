namespace ProductService.DTOs.BrandDTOs
{
    public class BrandDetailDTO
    {
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public string? BrandDescription { get; set; }
        public string? BrandImageUrl { get; set; }
        public string? BrandWebsiteUrl { get; set; }
        public DateTimeOffset? BrandCreatedAt { get; set; }
        public DateTimeOffset? BrandUpdatedAt { get; set; }
    }
}
