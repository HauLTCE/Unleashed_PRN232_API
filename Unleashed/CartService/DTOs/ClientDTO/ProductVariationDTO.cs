// File: DTOs/ClientDTOs/ProductVariationDTO.cs
namespace CartService.DTOs.ClientDTOs
{
    // DTO này được CartService sử dụng để hứng dữ liệu từ ProductService
    public class ProductVariationDTO
    {
        public int VariationId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } // << Quan trọng: Thêm trường này
        public decimal? VariationPrice { get; set; }
        public string VariationImage { get; set; }
        public string ColorName { get; set; } // Giả định ProductService trả về tên màu/size
        public string SizeName { get; set; }
    }
}