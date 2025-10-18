using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models
{
    [Table("wishlist")]
    [PrimaryKey(nameof(UserId), nameof(ProductId))]
    public partial class Wishlist
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }
    }
}