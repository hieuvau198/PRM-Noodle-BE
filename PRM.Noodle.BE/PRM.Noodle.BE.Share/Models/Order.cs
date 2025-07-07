using System;
using System.Collections.Generic;

namespace PRM.Noodle.BE.Share.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string? OrderStatus { get; set; }

    public decimal TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? Notes { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ICollection<OrderCombo> OrderCombos { get; set; } = new List<OrderCombo>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
