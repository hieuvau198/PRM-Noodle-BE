using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class combo
{
    public int combo_id { get; set; }

    public string combo_name { get; set; } = null!;

    public string? description { get; set; }

    public decimal combo_price { get; set; }

    public string? image_url { get; set; }

    public bool? is_available { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<combo_product> combo_products { get; set; } = new List<combo_product>();

    public virtual ICollection<order_combo> order_combos { get; set; } = new List<order_combo>();
}
