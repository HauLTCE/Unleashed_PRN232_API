using System.Text.Json.Serialization;

namespace CartService.DTOs.Client
{
    public class ProductVariationDTO
    {
        [JsonPropertyName("variationId")]
        public int VariationId { get; set; }

        [JsonPropertyName("productId")]
        public Guid ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("variationPrice")]
        public decimal? VariationPrice { get; set; }

        [JsonPropertyName("variationImage")]
        public string? VariationImage { get; set; }

        [JsonPropertyName("colorName")]
        public string ColorName { get; set; }

        [JsonPropertyName("sizeName")]
        public string SizeName { get; set; }
    }
}