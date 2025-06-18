using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class OrderItemTopping
{
    public int OrderItemToppingId { get; set; }

    public int OrderItemId { get; set; }

    public int ToppingId { get; set; }

    public int? Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual Topping Topping { get; set; } = null!;
}
