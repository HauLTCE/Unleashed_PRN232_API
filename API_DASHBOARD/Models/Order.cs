using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public Guid? UserId { get; set; }

    public int? OrderStatusId { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? ShippingMethodId { get; set; }

    public int? DiscountId { get; set; }

    public Guid? InchargeEmployeeId { get; set; }

    public DateTimeOffset? OrderDate { get; set; }

    public decimal? OrderTotalAmount { get; set; }

    public string? OrderTrackingNumber { get; set; }

    public string? OrderNote { get; set; }

    public string? OrderBillingAddress { get; set; }

    public DateTime? OrderExpectedDeliveryDate { get; set; }

    public string? OrderTransactionReference { get; set; }

    public decimal? OrderTax { get; set; }

    public DateTimeOffset? OrderCreatedAt { get; set; }

    public DateTimeOffset? OrderUpdatedAt { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual User? InchargeEmployee { get; set; }

    public virtual OrderStatus? OrderStatus { get; set; }

    public virtual ICollection<OrderVariationSingle> OrderVariationSingles { get; set; } = new List<OrderVariationSingle>();

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ShippingMethod? ShippingMethod { get; set; }

    public virtual User? User { get; set; }
}
