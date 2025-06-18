using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class daily_revenue
{
    public int revenue_id { get; set; }

    public DateOnly revenue_date { get; set; }

    public int? total_orders { get; set; }

    public decimal? total_revenue { get; set; }

    public decimal? cash_revenue { get; set; }

    public decimal? card_revenue { get; set; }

    public decimal? digital_wallet_revenue { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }
}
