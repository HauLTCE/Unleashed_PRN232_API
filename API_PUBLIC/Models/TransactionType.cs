using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class TransactionType
{
    public int TransactionTypeId { get; set; }

    public string? TransactionTypeName { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
