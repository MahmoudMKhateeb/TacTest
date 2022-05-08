using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DriverLocationLogs;
using TACHYON.DriverLocationLogs.dtos;

namespace TACHYON.AutoMapper.DriverLocationLogs
{
    public class DriverLocationLogProfile : Profile
    {
        public DriverLocationLogProfile()
        {
            CreateMap<CreateDriverLocationLogInput, DriverLocationLog>();

            CreateMap<DriverLocationLog, DriverLocationLogDto>()
                .ForMember(dst => dst.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dst => dst.Latitude, opt => opt.MapFrom(src => src.Location.Y));
        }
    }
}