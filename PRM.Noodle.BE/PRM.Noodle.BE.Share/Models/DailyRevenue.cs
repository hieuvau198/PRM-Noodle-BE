using System;
using System.Collections.Generic;

namespace PRM.Noodle.BE.Share.Models;

public partial class DailyRevenue
{
    public int RevenueId { get; set; }

    public DateOnly RevenueDate { get; set; }

    public int? TotalOrders { get; set; }

    public decimal? TotalRevenue { get; set; }

    public decimal? CashRevenue { get; set; }

    public decimal? CardRevenue { get; set; }

    public decimal? DigitalWalletRevenue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
