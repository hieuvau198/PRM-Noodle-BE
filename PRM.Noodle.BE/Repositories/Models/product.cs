using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class product
{
    public int product_id { get; set; }

    public string product_name { get; set; } = null!;

    public string? description { get; set; }

    public decimal base_price { get; set; }

    public string? image_url { get; set; }

    public bool? is_available { get; set; }

    public string? spice_level { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<combo_product> combo_products { get; set; } = new List<combo_product>();

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();
}
