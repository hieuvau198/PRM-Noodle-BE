using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Reports.Models
{
    public class DailyRevenueReportDto
    {
        public DateOnly RevenueDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CashRevenue { get; set; }
        public decimal CardRevenue { get; set; }
        public decimal DigitalWalletRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class MonthlyRevenueReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CashRevenue { get; set; }
        public decimal CardRevenue { get; set; }
        public decimal DigitalWalletRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalDays { get; set; }
        public decimal AverageDailyRevenue { get; set; }
        public IEnumerable<DailyRevenueReportDto> DailyBreakdown { get; set; }
    }

    public class YearlyRevenueReportDto
    {
        public int Year { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CashRevenue { get; set; }
        public decimal CardRevenue { get; set; }
        public decimal DigitalWalletRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal AverageMonthlyRevenue { get; set; }
        public IEnumerable<MonthlyRevenueReportDto> MonthlyBreakdown { get; set; }
    }

    public class MonthlyRevenueSimpleDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyRevenueSimpleDto
    {
        public DateOnly Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DailyOrderCountDto
    {
        public DateOnly Date { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalOrdered { get; set; }
    }

    public class OrderStatusPortionDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
