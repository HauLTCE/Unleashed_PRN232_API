using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs.Cart
{
    public class CreateCartDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int VariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CartQuantity { get; set; }
    }
}