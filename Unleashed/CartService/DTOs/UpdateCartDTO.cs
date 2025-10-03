using System.ComponentModel.DataAnnotations;

namespace CartService.Dtos
{
    public class UpdateCartDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CartQuantity { get; set; }
    }
}
