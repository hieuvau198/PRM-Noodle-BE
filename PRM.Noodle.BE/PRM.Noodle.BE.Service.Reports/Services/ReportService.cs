using AutoMapper;
using PRM.Noodle.BE.Service.Reports.Models;
using PRM.Noodle.BE.Share.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<DailyRevenueReportDto> GetDailyRevenueAsync(DateOnly date)
        {
            var dailyRevenue = await _uow.DailyRevenues
                .FindAsync(dr => dr.RevenueDate == date);

            var revenue = dailyRevenue.FirstOrDefault();

            if (revenue == null)
            {
                return new DailyRevenueReportDto
                {
                    RevenueDate = date,
                    TotalOrders = 0,
                    TotalRevenue = 0,
                    CashRevenue = 0,
                    CardRevenue = 0,
                    DigitalWalletRevenue = 0,
                    AverageOrderValue = 0
                };
            }

            return new DailyRevenueReportDto
            {
                RevenueDate = revenue.RevenueDate,
                TotalOrders = revenue.TotalOrders ?? 0,
                TotalRevenue = revenue.TotalRevenue ?? 0,
                CashRevenue = revenue.CashRevenue ?? 0,
                CardRevenue = revenue.CardRevenue ?? 0,
                DigitalWalletRevenue = revenue.DigitalWalletRevenue ?? 0,
                AverageOrderValue = (revenue.TotalOrders ?? 0) > 0 ?
                    (revenue.TotalRevenue ?? 0) / (revenue.TotalOrders ?? 1) : 0
            };
        }

        public async Task<MonthlyRevenueReportDto> GetMonthlyRevenueAsync(int year, int month)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var monthlyRevenues = await _uow.DailyRevenues
                .FindAsync(dr => dr.RevenueDate >= startDate && dr.RevenueDate <= endDate);

            var revenues = monthlyRevenues.ToList();
            var dailyBreakdown = revenues.Select(r => new DailyRevenueReportDto
            {
                RevenueDate = r.RevenueDate,
                TotalOrders = r.TotalOrders ?? 0,
                TotalRevenue = r.TotalRevenue ?? 0,
                CashRevenue = r.CashRevenue ?? 0,
                CardRevenue = r.CardRevenue ?? 0,
                DigitalWalletRevenue = r.DigitalWalletRevenue ?? 0,
                AverageOrderValue = (r.TotalOrders ?? 0) > 0 ?
                    (r.TotalRevenue ?? 0) / (r.TotalOrders ?? 1) : 0
            }).OrderBy(d => d.RevenueDate);

            var totalOrders = revenues.Sum(r => r.TotalOrders ?? 0);
            var totalRevenue = revenues.Sum(r => r.TotalRevenue ?? 0);
            var totalDays = DateTime.DaysInMonth(year, month);

            return new MonthlyRevenueReportDto
            {
                Year = year,
                Month = month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                CashRevenue = revenues.Sum(r => r.CashRevenue ?? 0),
                CardRevenue = revenues.Sum(r => r.CardRevenue ?? 0),
                DigitalWalletRevenue = revenues.Sum(r => r.DigitalWalletRevenue ?? 0),
                AverageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0,
                TotalDays = totalDays,
                AverageDailyRevenue = totalDays > 0 ? totalRevenue / totalDays : 0,
                DailyBreakdown = dailyBreakdown
            };
        }

        public async Task<YearlyRevenueReportDto> GetYearlyRevenueAsync(int year)
        {
            var startDate = new DateOnly(year, 1, 1);
            var endDate = new DateOnly(year, 12, 31);

            var yearlyRevenues = await _uow.DailyRevenues
                .FindAsync(dr => dr.RevenueDate >= startDate && dr.RevenueDate <= endDate);

            var revenues = yearlyRevenues.ToList();

            // Group by month for monthly breakdown
            var monthlyBreakdown = new List<MonthlyRevenueReportDto>();
            for (int month = 1; month <= 12; month++)
            {
                var monthlyReport = await GetMonthlyRevenueAsync(year, month);
                monthlyBreakdown.Add(monthlyReport);
            }

            var totalOrders = revenues.Sum(r => r.TotalOrders ?? 0);
            var totalRevenue = revenues.Sum(r => r.TotalRevenue ?? 0);

            return new YearlyRevenueReportDto
            {
                Year = year,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                CashRevenue = revenues.Sum(r => r.CashRevenue ?? 0),
                CardRevenue = revenues.Sum(r => r.CardRevenue ?? 0),
                DigitalWalletRevenue = revenues.Sum(r => r.DigitalWalletRevenue ?? 0),
                AverageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0,
                AverageMonthlyRevenue = totalRevenue / 12,
                MonthlyBreakdown = monthlyBreakdown
            };
        }

        public async Task<IEnumerable<DailyRevenueReportDto>> GetDailyRevenueRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            var dailyRevenues = await _uow.DailyRevenues
                .FindAsync(dr => dr.RevenueDate >= startDate && dr.RevenueDate <= endDate);

            return dailyRevenues.Select(r => new DailyRevenueReportDto
            {
                RevenueDate = r.RevenueDate,
                TotalOrders = r.TotalOrders ?? 0,
                TotalRevenue = r.TotalRevenue ?? 0,
                CashRevenue = r.CashRevenue ?? 0,
                CardRevenue = r.CardRevenue ?? 0,
                DigitalWalletRevenue = r.DigitalWalletRevenue ?? 0,
                AverageOrderValue = (r.TotalOrders ?? 0) > 0 ?
                    (r.TotalRevenue ?? 0) / (r.TotalOrders ?? 1) : 0
            }).OrderBy(d => d.RevenueDate);
        }

        public async Task<IEnumerable<MonthlyRevenueSimpleDto>> GetLast6MonthsRevenueAsync()
        {
            var now = DateTime.Now;
            var results = new List<MonthlyRevenueSimpleDto>();

            for (int i = 5; i >= 0; i--)
            {
                var targetDate = now.AddMonths(-i);
                var startDate = new DateOnly(targetDate.Year, targetDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var monthlyRevenues = await _uow.DailyRevenues
                    .FindAsync(dr => dr.RevenueDate >= startDate && dr.RevenueDate <= endDate);

                var totalRevenue = monthlyRevenues.Sum(r => r.TotalRevenue ?? 0);

                results.Add(new MonthlyRevenueSimpleDto
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(targetDate.Month),
                    TotalRevenue = totalRevenue
                });
            }

            return results;
        }

        public async Task<IEnumerable<DailyRevenueSimpleDto>> GetLast7DaysRevenueAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var startDate = today.AddDays(-6);
            var endDate = today;

            var dailyRevenues = await _uow.DailyRevenues
                .FindAsync(dr => dr.RevenueDate >= startDate && dr.RevenueDate <= endDate);

            var revenueDict = dailyRevenues.ToDictionary(dr => dr.RevenueDate, dr => dr.TotalRevenue ?? 0);

            var results = new List<DailyRevenueSimpleDto>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                results.Add(new DailyRevenueSimpleDto
                {
                    Date = date,
                    TotalRevenue = revenueDict.GetValueOrDefault(date, 0)
                });
            }

            return results;
        }

        public async Task<IEnumerable<DailyOrderCountDto>> GetLast7DaysOrderCountAsync()
        {
            var today = DateTime.Now.Date;
            var startDate = today.AddDays(-6);
            var endDate = today.AddDays(1); // Include today

            var orders = await _uow.Orders
                .FindAsync(o => o.OrderDate >= startDate && o.OrderDate < endDate);

            var orderCounts = orders
                .GroupBy(o => DateOnly.FromDateTime(o.OrderDate.Value))
                .ToDictionary(g => g.Key, g => g.Count());

            var results = new List<DailyOrderCountDto>();
            for (var date = DateOnly.FromDateTime(startDate); date <= DateOnly.FromDateTime(today); date = date.AddDays(1))
            {
                results.Add(new DailyOrderCountDto
                {
                    Date = date,
                    OrderCount = orderCounts.GetValueOrDefault(date, 0)
                });
            }

            return results;
        }

        public async Task<int> GetTodayOrderCountAsync()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            var orders = await _uow.Orders
                .FindAsync(o => o.OrderDate >= today && o.OrderDate < tomorrow);

            return orders.Count();
        }

        public async Task<IEnumerable<TopProductDto>> GetTop5ProductsAsync()
        {
            var orderItems = await _uow.OrderItems.GetAllAsync();
            var products = await _uow.Products.GetAllAsync();

            var topProducts = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOrdered = g.Sum(oi => oi.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalOrdered)
                .Take(5)
                .ToList();

            var productDict = products.ToDictionary(p => p.ProductId, p => p.ProductName);

            return topProducts.Select(tp => new TopProductDto
            {
                ProductId = tp.ProductId,
                ProductName = productDict.GetValueOrDefault(tp.ProductId, "Unknown Product"),
                TotalOrdered = tp.TotalOrdered
            });
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            var orders = await _uow.Orders.GetAllAsync();
            return orders.Count();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var orders = await _uow.Orders.GetAllAsync();
            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<TopProductDto> GetMostOrderedProductAsync()
        {
            var orderItems = await _uow.OrderItems.GetAllAsync();
            var products = await _uow.Products.GetAllAsync();

            var mostOrderedProduct = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalOrdered = g.Sum(oi => oi.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalOrdered)
                .FirstOrDefault();

            if (mostOrderedProduct == null)
            {
                return new TopProductDto
                {
                    ProductId = 0,
                    ProductName = "No orders found",
                    TotalOrdered = 0
                };
            }

            var product = products.FirstOrDefault(p => p.ProductId == mostOrderedProduct.ProductId);

            return new TopProductDto
            {
                ProductId = mostOrderedProduct.ProductId,
                ProductName = product?.ProductName ?? "Unknown Product",
                TotalOrdered = mostOrderedProduct.TotalOrdered
            };
        }
    }
}
