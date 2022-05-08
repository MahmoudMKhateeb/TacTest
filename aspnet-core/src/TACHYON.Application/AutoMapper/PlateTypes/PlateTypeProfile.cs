using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Trucks.PlateTypes;
using TACHYON.Trucks.PlateTypes.Dtos;

namespace TACHYON.AutoMapper.PlateTypes
{
    public class PlateTypeProfile : Profile
    {
        public PlateTypeProfile()
        {
            CreateMap<CreateOrEditPlateTypeDto, PlateType>()
                .ForMember(dest => dest.Translations,
                    opt => opt.MapFrom(src => src.Translations))
                .ReverseMap();

            //CreateMap<PlateTypeDto, PlateType>()
            //    .ReverseMap();

            CreateMap<PlateTypeTranslation, PlateTypeTranslationDto>().ReverseMap();

            CreateMap<PlateType, GetPlateTypeForViewDto>()
                .ForMember(dest => dest.PlateType,
                    opt => opt.MapFrom(src => src));
        }
    }
}