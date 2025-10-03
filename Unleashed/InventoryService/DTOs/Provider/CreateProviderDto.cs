using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.Provider
{
    public class CreateProviderDto
    {
        [Required]
        [StringLength(255)]
        public string? ProviderName { get; set; }

        [StringLength(255)]
        public string? ProviderImageUrl { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? ProviderEmail { get; set; }

        [StringLength(12)]
        public string? ProviderPhone { get; set; }

        [StringLength(255)]
        public string? ProviderAddress { get; set; }
    }
}