namespace CartService.DTOs.Client
{
    public class VariationDTO
    {
        public int Id { get; set; }
        public decimal VariationPrice { get; set; }
        public string VariationImage { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
    }
}
