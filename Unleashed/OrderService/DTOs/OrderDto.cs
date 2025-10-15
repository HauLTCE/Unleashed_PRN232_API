using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Dtos
{
    public class OrderDto
    {
        public string OrderId { get; set; }
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

        public string? OrderStatusName { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? ShippingMethodName { get; set; }
        public ICollection<OrderVariationSingleDto> OrderVariationSingles { get; set; }
    }

    public class CreateOrderDto
    {
        public Guid? UserId { get; set; }
        [Required]
        public int? OrderStatusId { get; set; }
        [Required]
        public int? PaymentMethodId { get; set; }
        [Required]
        public int? ShippingMethodId { get; set; }
        public int? DiscountId { get; set; }
        public decimal? OrderTotalAmount { get; set; }
        [Required]
        public string? OrderBillingAddress { get; set; }
        public string? OrderNote { get; set; }

        public ICollection<CreateOrderVariationSingleDto> OrderVariationSingles { get; set; }
    }

    public class UpdateOrderDto
    {
        public int? OrderStatusId { get; set; }
        public Guid? InchargeEmployeeId { get; set; }
        public string? OrderTrackingNumber { get; set; }
        public DateTime? OrderExpectedDeliveryDate { get; set; }
        public string? OrderNote { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
    }
}