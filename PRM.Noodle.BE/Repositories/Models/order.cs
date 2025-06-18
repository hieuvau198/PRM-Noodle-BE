using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class order
{
    public int order_id { get; set; }

    public int user_id { get; set; }

    public string? order_status { get; set; }

    public decimal total_amount { get; set; }

    public string? payment_method { get; set; }

    public string? payment_status { get; set; }

    public string? delivery_address { get; set; }

    public string? notes { get; set; }

    public DateTime? order_date { get; set; }

    public DateTime? confirmed_at { get; set; }

    public DateTime? completed_at { get; set; }

    public virtual ICollection<order_combo> order_combos { get; set; } = new List<order_combo>();

    public virtual ICollection<order_item> order_items { get; set; } = new List<order_item>();

    public virtual user user { get; set; } = null!;
}
