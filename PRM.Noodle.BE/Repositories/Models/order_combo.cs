using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class order_combo
{
    public int order_combo_id { get; set; }

    public int order_id { get; set; }

    public int combo_id { get; set; }

    public int? quantity { get; set; }

    public decimal unit_price { get; set; }

    public decimal subtotal { get; set; }

    public virtual combo combo { get; set; } = null!;

    public virtual order order { get; set; } = null!;
}
