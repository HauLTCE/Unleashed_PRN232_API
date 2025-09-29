using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Table("order")]
public partial class Order
{
    [Key]
    [Column("order_id")]
    [StringLength(255)]
    public string OrderId { get; set; } = null!;

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("order_status_id")]
    public int? OrderStatusId { get; set; }

    [Column("payment_method_id")]
    public int? PaymentMethodId { get; set; }

    [Column("shipping_method_id")]
    public int? ShippingMethodId { get; set; }

    [Column("discount_id")]
    public int? DiscountId { get; set; }

    [Column("incharge_employee_id")]
    public Guid? InchargeEmployeeId { get; set; }

    [Column("order_date")]
    public DateTimeOffset? OrderDate { get; set; }

    [Column("order_total_amount", TypeName = "decimal(22, 2)")]
    public decimal? OrderTotalAmount { get; set; }

    [Column("order_tracking_number")]
    [StringLength(50)]
    public string? OrderTrackingNumber { get; set; }

    [Column("order_note")]
    public string? OrderNote { get; set; }

    [Column("order_billing_address")]
    [StringLength(255)]
    public string? OrderBillingAddress { get; set; }

    [Column("order_expected_delivery_date")]
    public DateTime? OrderExpectedDeliveryDate { get; set; }

    [Column("order_transaction_reference")]
    [StringLength(255)]
    public string? OrderTransactionReference { get; set; }

    [Column("order_tax", TypeName = "numeric(3, 2)")]
    public decimal? OrderTax { get; set; }

    [Column("order_created_at")]
    public DateTimeOffset? OrderCreatedAt { get; set; }

    [Column("order_updated_at")]
    public DateTimeOffset? OrderUpdatedAt { get; set; }

    [ForeignKey("OrderStatusId")]
    [InverseProperty("Orders")]
    public virtual OrderStatus? OrderStatus { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderVariationSingle> OrderVariationSingles { get; set; } = new List<OrderVariationSingle>();

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Orders")]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [ForeignKey("ShippingMethodId")]
    [InverseProperty("Orders")]
    public virtual ShippingMethod? ShippingMethod { get; set; }
}
