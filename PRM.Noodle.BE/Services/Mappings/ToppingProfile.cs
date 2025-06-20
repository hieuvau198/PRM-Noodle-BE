using AutoMapper;
using Repositories.Models;
using Services.DTOs.Topping;

public class ToppingProfile : Profile
{
    public ToppingProfile()
    {
        CreateMap<Topping, ToppingDto>().ReverseMap();
        CreateMap<CreateToppingDto, Topping>();
        CreateMap<UpdateToppingDto, Topping>();
    }
}