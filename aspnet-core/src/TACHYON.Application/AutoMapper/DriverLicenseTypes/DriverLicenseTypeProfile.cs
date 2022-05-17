using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DriverLicenseTypes;
using TACHYON.DriverLicenseTypes.Dtos;

namespace TACHYON.AutoMapper.DriverLicenseTypes
{
    public class DriverLicenseTypeProfile : Profile
    {
        public DriverLicenseTypeProfile()
        {
            CreateMap<CreateOrEditDriverLicenseTypeDto, DriverLicenseType>().ReverseMap();

            CreateMap<DriverLicenseTypeTranslationDto, DriverLicenseTypeTranslation>()
                .ForMember(x=> x.DisplayName,x=> x.MapFrom(i=> i.Name)).ReverseMap();
        }
    }
}