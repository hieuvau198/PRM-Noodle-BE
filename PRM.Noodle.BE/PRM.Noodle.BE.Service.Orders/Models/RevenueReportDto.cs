using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Orders.Models
{
    public class RevenueReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailyRevenueDto> DailyRevenueBreakdown { get; set; } = new List<DailyRevenueDto>();
        public List<OrderStatusSummaryDto> OrderStatusSummary { get; set; } = new List<OrderStatusSummaryDto>();
    }
}
