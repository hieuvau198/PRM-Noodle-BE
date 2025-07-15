using AutoMapper;
using PRM.Noodle.BE.Service.Reports.Models;
using PRM.Noodle.BE.Share.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Reports.Mappings
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {
            // Map DailyRevenue entity to DailyRevenueReportDto
            CreateMap<DailyRevenue, DailyRevenueReportDto>()
                .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.TotalOrders ?? 0))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.TotalRevenue ?? 0))
                .ForMember(dest => dest.CashRevenue, opt => opt.MapFrom(src => src.CashRevenue ?? 0))
                .ForMember(dest => dest.CardRevenue, opt => opt.MapFrom(src => src.CardRevenue ?? 0))
                .ForMember(dest => dest.DigitalWalletRevenue, opt => opt.MapFrom(src => src.DigitalWalletRevenue ?? 0))
                .ForMember(dest => dest.AverageOrderValue, opt => opt.Ignore()); // You calculate this separately

            // If you want, you can add similar mappings for MonthlyRevenueReportDto and YearlyRevenueReportDto
            // But as your current DTOs are composed from daily data, direct mapping for those is optional
        }
    }
}
