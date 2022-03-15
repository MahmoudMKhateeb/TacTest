using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.UnitOfMeasures;
using TACHYON.UnitOfMeasures.Dtos;

namespace TACHYON.AutoMapper.UnitOfMeasures
{
    public class UnitOfMeasureProfile : Profile
    {
        public UnitOfMeasureProfile()
        {
            CreateMap<CreateOrEditUnitOfMeasureDto, UnitOfMeasure>()
                .ForMember(x => x.Translations, opt => opt.MapFrom(src => src.Translations)).ReverseMap();
            CreateMap<UnitOfmeasureTranslationDto, UnitOfMeasureTranslation>().ReverseMap();

        }
    }
}