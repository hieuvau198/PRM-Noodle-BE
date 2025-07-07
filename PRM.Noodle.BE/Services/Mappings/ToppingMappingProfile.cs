using AutoMapper;
using PRM.Noodle.BE.Share.Models;
using Services.DTOs.Topping;

namespace Services.Mappings
{
    public class ToppingMappingProfile : Profile
    {
        public ToppingMappingProfile()
        {
            CreateMap<Topping, ToppingDto>().ReverseMap();
            CreateMap<CreateToppingDto, Topping>();
            CreateMap<UpdateToppingDto, Topping>();
        }
    }
}