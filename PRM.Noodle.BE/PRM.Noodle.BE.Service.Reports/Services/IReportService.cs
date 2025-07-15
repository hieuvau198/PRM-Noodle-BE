using PRM.Noodle.BE.Service.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Reports.Services
{
    public interface IReportService
    {
        Task<DailyRevenueReportDto> GetDailyRevenueAsync(DateOnly date);
        Task<MonthlyRevenueReportDto> GetMonthlyRevenueAsync(int year, int month);
        Task<YearlyRevenueReportDto> GetYearlyRevenueAsync(int year);
        Task<IEnumerable<DailyRevenueReportDto>> GetDailyRevenueRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<MonthlyRevenueSimpleDto>> GetLast6MonthsRevenueAsync();
        Task<IEnumerable<DailyRevenueSimpleDto>> GetLast7DaysRevenueAsync();
        Task<IEnumerable<DailyOrderCountDto>> GetLast7DaysOrderCountAsync();
        Task<int> GetTodayOrderCountAsync();
        Task<IEnumerable<TopProductDto>> GetTop5ProductsAsync();
        Task<int> GetTotalOrdersAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<TopProductDto> GetMostOrderedProductAsync();
    }
}
