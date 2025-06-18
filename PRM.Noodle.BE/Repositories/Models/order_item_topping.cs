using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class order_item_topping
{
    public int order_item_topping_id { get; set; }

    public int order_item_id { get; set; }

    public int topping_id { get; set; }

    public int? quantity { get; set; }

    public decimal unit_price { get; set; }

    public decimal subtotal { get; set; }

    public virtual order_item order_item { get; set; } = null!;

    public virtual topping topping { get; set; } = null!;
}
