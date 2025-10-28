namespace InventoryService.DTOs.Internal
{
    public class TransactionCardDTO
    {
        public int TransactionId { get; set; }
        public string? VariationImage { get; set; }
        public string? ProductName { get; set; }
        public string? StockName { get; set; }
        public string? TransactionTypeName { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public string? SizeName { get; set; }
        public string? ColorName { get; set; }
        public string? ColorHexCode { get; set; }
        public decimal? TransactionProductPrice { get; set; }
        public int? TransactionQuantity { get; set; }
        public DateTimeOffset? TransactionDate { get; set; }
        public string? InchargeEmployeeUsername { get; set; }
        public string? ProviderName { get; set; }
    }
}