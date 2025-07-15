using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRM.Noodle.BE.Service.Reports.Models;
using PRM.Noodle.BE.Service.Reports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Reports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("daily")]
        public async Task<ActionResult<DailyRevenueReportDto>> GetDailyRevenue([FromQuery] DateOnly date)
        {
            try
            {
                var result = await _reportService.GetDailyRevenueAsync(date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("daily/today")]
        public async Task<ActionResult<DailyRevenueReportDto>> GetTodayRevenue()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var result = await _reportService.GetDailyRevenueAsync(today);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<MonthlyRevenueReportDto>> GetMonthlyRevenue(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    return BadRequest(new { message = "Month must be between 1 and 12" });
                }

                if (year < 2000 || year > DateTime.Now.Year + 1)
                {
                    return BadRequest(new { message = "Invalid year" });
                }

                var result = await _reportService.GetMonthlyRevenueAsync(year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("monthly/current")]
        public async Task<ActionResult<MonthlyRevenueReportDto>> GetCurrentMonthRevenue()
        {
            try
            {
                var now = DateTime.Now;
                var result = await _reportService.GetMonthlyRevenueAsync(now.Year, now.Month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("yearly")]
        public async Task<ActionResult<YearlyRevenueReportDto>> GetYearlyRevenue([FromQuery] int year)
        {
            try
            {
                if (year < 2000 || year > DateTime.Now.Year + 1)
                {
                    return BadRequest(new { message = "Invalid year" });
                }

                var result = await _reportService.GetYearlyRevenueAsync(year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("yearly/current")]
        public async Task<ActionResult<YearlyRevenueReportDto>> GetCurrentYearRevenue()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var result = await _reportService.GetYearlyRevenueAsync(currentYear);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("daily/range")]
        public async Task<ActionResult<IEnumerable<DailyRevenueReportDto>>> GetDailyRevenueRange(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var result = await _reportService.GetDailyRevenueRangeAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("revenue/last6months")]
        public async Task<ActionResult<IEnumerable<MonthlyRevenueSimpleDto>>> GetLast6MonthsRevenue()
        {
            try
            {
                var result = await _reportService.GetLast6MonthsRevenueAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("revenue/last7days")]
        public async Task<ActionResult<IEnumerable<DailyRevenueSimpleDto>>> GetLast7DaysRevenue()
        {
            try
            {
                var result = await _reportService.GetLast7DaysRevenueAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders/last7days")]
        public async Task<ActionResult<IEnumerable<DailyOrderCountDto>>> GetLast7DaysOrderCount()
        {
            try
            {
                var result = await _reportService.GetLast7DaysOrderCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders/today")]
        public async Task<ActionResult<int>> GetTodayOrderCount()
        {
            try
            {
                var result = await _reportService.GetTodayOrderCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("products/top5")]
        public async Task<ActionResult<IEnumerable<TopProductDto>>> GetTop5Products()
        {
            try
            {
                var result = await _reportService.GetTop5ProductsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
