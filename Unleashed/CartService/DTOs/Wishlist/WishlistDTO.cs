using CartService.DTOs.Client;

namespace CartService.DTOs.Wishlist
{
    public class WishlistDTO
    {
        public Guid UserId { get; set; }
        public List<WishlistProductInfoDTO> Products { get; set; }
    }
}