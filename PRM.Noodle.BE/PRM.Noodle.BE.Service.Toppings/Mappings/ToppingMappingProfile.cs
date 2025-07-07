using AutoMapper;
using PRM.Noodle.BE.Service.Toppings.Models;
using PRM.Noodle.BE.Share.Models;

namespace PRM.Noodle.BE.Service.Toppings.Mappings
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
