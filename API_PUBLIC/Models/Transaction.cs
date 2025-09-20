using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class Transaction
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

    public virtual User? InchargeEmployee { get; set; }

    public virtual Provider? Provider { get; set; }

    public virtual Stock? Stock { get; set; }

    public virtual TransactionType? TransactionType { get; set; }

    public virtual Variation? Variation { get; set; }
}
