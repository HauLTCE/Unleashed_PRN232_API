using System.ComponentModel.DataAnnotations;

namespace CartService.Dtos
{
    public class CreateCartDTO
    {
        public Guid UserId { get; set; }
        public int VariationId { get; set; }
        public int? CartQuantity { get; set; }
    }
}