using System.ComponentModel.DataAnnotations;

namespace DiscountService.DTOs
{
    public class CreateDiscountDto
    {
        [Required]
        public int? DiscountStatusId { get; set; }

        [Required]
        public int? DiscountTypeId { get; set; }

        [Required]
        [StringLength(20)]
        public string DiscountCode { get; set; }

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        public decimal? DiscountValue { get; set; }

        [StringLength(255)]
        public string? DiscountDescription { get; set; }

        //public int? DiscountRankRequirement { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        public decimal? DiscountMinimumOrderValue { get; set; }

        [Range(0, (double)decimal.MaxValue)]
        public decimal? DiscountMaximumValue { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? DiscountUsageLimit { get; set; }

        [Required]
        public DateTimeOffset? DiscountStartDate { get; set; }

        [Required]
        public DateTimeOffset? DiscountEndDate { get; set; }
    }
}
