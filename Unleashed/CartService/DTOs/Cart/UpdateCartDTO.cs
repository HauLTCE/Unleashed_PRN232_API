using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs.Cart
{
    public class UpdateCartDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CartQuantity { get; set; }
    }
}
