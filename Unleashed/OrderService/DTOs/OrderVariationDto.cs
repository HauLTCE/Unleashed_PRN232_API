using System.ComponentModel.DataAnnotations;

namespace OrderService.Dtos
{
    public class OrderVariationDto
    {
        public int VariationId { get; set; }
        public int? SaleId { get; set; }
        public int? Quantity { get; set; }

        public decimal VariationPriceAtPurchase { get; set; }
    }

    public class CreateOrderVariationDto
    {
        [Required]
        public int VariationId { get; set; }

        [Required]
        public int? Quantity { get; set; }

        public int? SaleId { get; set; }
        [Required]
        public decimal VariationPriceAtPurchase { get; set; }
    }
}