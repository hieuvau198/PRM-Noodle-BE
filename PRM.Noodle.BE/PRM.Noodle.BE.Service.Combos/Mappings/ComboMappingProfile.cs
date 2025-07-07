using AutoMapper;
using PRM.Noodle.BE.Service.Combos.Models;
using PRM.Noodle.BE.Share.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Combos.Mappings
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
