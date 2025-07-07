using AutoMapper;
using PRM.Noodle.BE.Share.Models;
using Services.DTOs.Combo;

namespace Services.Mappings
{
    public class ComboMappingProfile : Profile
    {
        public ComboMappingProfile()
        {
            CreateMap<Combo, ComboDto>().ReverseMap();
            CreateMap<CreateComboDto, Combo>();
            CreateMap<UpdateComboDto, Combo>();
        }
    }
}
