// DTOs/DiscountViewDto.cs
namespace DiscountService.DTOs
{
    public class DiscountViewDto
    {
        public int DiscountId { get; set; }

        // --- ID và Tên của các thực thể liên quan ---
        public int? DiscountStatusId { get; set; }
        public string? DiscountStatusName { get; set; } // Lấy từ navigation property

        public int? DiscountTypeId { get; set; }
        public string? DiscountTypeName { get; set; } // Lấy từ navigation property

        // --- Thông tin cơ bản của Discount ---
        public string DiscountCode { get; set; } = null!;
        public decimal? DiscountValue { get; set; }
        public string? DiscountDescription { get; set; }

        // --- Các điều kiện và giới hạn ---
        public int? DiscountRankRequirement { get; set; } // Thêm trường Rank
        public decimal? DiscountMinimumOrderValue { get; set; }
        public decimal? DiscountMaximumValue { get; set; }
        public int? DiscountUsageLimit { get; set; }

        // --- Thời gian hiệu lực ---
        public DateTimeOffset? DiscountStartDate { get; set; }
        public DateTimeOffset? DiscountEndDate { get; set; }

        // --- Các mốc thời gian hệ thống ---
        public DateTimeOffset? DiscountCreatedAt { get; set; }
        public DateTimeOffset? DiscountUpdatedAt { get; set; }

        // --- Thông tin về lượt sử dụng ---
        public int? DiscountUsageCount { get; set; }
        // Thay vì trả về cả một danh sách UserDiscount, ta chỉ cần số lượng
        public int UsersAssignedCount { get; set; }
    }
}