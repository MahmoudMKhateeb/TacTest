using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Dtos;

namespace TACHYON.AutoMapper.AddressBook
{
    public class FacilityProfile : Profile
    {
        public FacilityProfile()
        {
            CreateMap<Facility, CreateOrEditFacilityDto>()
                .ForMember(dst => dst.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dst => dst.Latitude, opt => opt.MapFrom(src => src.Location.Y))
                .ForPath(dst => dst.FacilityWorkingHours, opt => opt.MapFrom(src => src.FacilityWorkingHours));

            CreateMap<FacilityWorkingHour, FacilityWorkingHourDto>();
            CreateMap<CreateOrEditFacilityWorkingHourDto, FacilityWorkingHour>().ReverseMap();
        }
    }
}
