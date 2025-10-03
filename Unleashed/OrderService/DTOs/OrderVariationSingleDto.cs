using System.ComponentModel.DataAnnotations;

namespace OrderService.Dtos
{
    public class OrderVariationSingleDto
    {
        public int VariationSingleId { get; set; }
        public int? SaleId { get; set; }
        public decimal VariationPriceAtPurchase { get; set; }
    }

    public class CreateOrderVariationSingleDto
    {
        [Required]
        public int VariationSingleId { get; set; }
        public int? SaleId { get; set; }
        [Required]
        public decimal VariationPriceAtPurchase { get; set; }
    }
}