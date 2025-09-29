using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Models;

[Table("provider")]
public partial class Provider
{
    [Key]
    [Column("provider_id")]
    public int ProviderId { get; set; }

    [Column("provider_name")]
    [StringLength(255)]
    public string? ProviderName { get; set; }

    [Column("provider_image_url")]
    [StringLength(255)]
    public string? ProviderImageUrl { get; set; }

    [Column("provider_email")]
    [StringLength(255)]
    public string? ProviderEmail { get; set; }

    [Column("provider_phone")]
    [StringLength(12)]
    public string? ProviderPhone { get; set; }

    [Column("provider_address")]
    [StringLength(255)]
    public string? ProviderAddress { get; set; }

    [Column("provider_created_at")]
    public DateTimeOffset? ProviderCreatedAt { get; set; }

    [Column("provider_updated_at")]
    public DateTimeOffset? ProviderUpdatedAt { get; set; }

    [InverseProperty("Provider")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
