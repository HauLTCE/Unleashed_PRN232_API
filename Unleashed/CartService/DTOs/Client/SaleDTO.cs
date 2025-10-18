namespace CartService.DTOs.Client
{
    public class SaleDTO
    {
        public decimal SalePrice { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
    }
}