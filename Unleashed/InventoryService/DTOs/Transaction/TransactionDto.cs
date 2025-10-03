namespace InventoryService.DTOs.Transaction
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int? StockId { get; set; }
        public int? VariationId { get; set; }
        public int? ProviderId { get; set; }
        public Guid? InchargeEmployeeId { get; set; }
        public int? TransactionTypeId { get; set; }
        public int? TransactionQuantity { get; set; }
        public DateTimeOffset? TransactionDate { get; set; }
        public decimal? TransactionProductPrice { get; set; }
    }
}