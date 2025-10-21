namespace ProductService.DTOs.SaleDTOs
{
    public class SaleDTO
    {
        public int SaleId { get; set; }
        public int? SaleTypeId { get; set; }
        public int? SaleStatusId { get; set; }
        public decimal? SaleValue { get; set; }
        public DateTimeOffset? SaleStartDate { get; set; }
        public DateTimeOffset? SaleEndDate { get; set; }
    }
}
