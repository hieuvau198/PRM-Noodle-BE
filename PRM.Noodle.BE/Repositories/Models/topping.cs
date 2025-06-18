using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class topping
{
    public int topping_id { get; set; }

    public string topping_name { get; set; } = null!;

    public decimal price { get; set; }

    public string? description { get; set; }

    public bool? is_available { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<order_item_topping> order_item_toppings { get; set; } = new List<order_item_topping>();
}
