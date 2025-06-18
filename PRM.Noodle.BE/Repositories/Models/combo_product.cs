using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class combo_product
{
    public int combo_product_id { get; set; }

    public int combo_id { get; set; }

    public int product_id { get; set; }

    public int? quantity { get; set; }

    public virtual combo combo { get; set; } = null!;

    public virtual product product { get; set; } = null!;
}
