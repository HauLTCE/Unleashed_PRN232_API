namespace InventoryService.DTOs.Provider
{
    public class ProviderDto
    {
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderImageUrl { get; set; }
        public string? ProviderEmail { get; set; }
        public string? ProviderPhone { get; set; }
        public string? ProviderAddress { get; set; }
    }
}