namespace ProductService.DTOs.CategoryDTOs
{
    public class CategoryDetailDTO
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        public string? CategoryImageUrl { get; set; }
        public DateTimeOffset? CategoryCreatedAt { get; set; }
        public DateTimeOffset? CategoryUpdatedAt { get; set; }
    }
}
