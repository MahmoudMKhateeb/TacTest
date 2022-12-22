using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.Dedicated;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;
using TACHYON.Shipping.ShippingRequests.Dtos.TruckAttendance;

namespace TACHYON.AutoMapper.Shipping
{
    public class DedicatedShippingRequestProfile : Profile
    {
        public DedicatedShippingRequestProfile()
        {
            CreateMap<CreateOrEditTruckAttendanceDto, DedicatedShippingRequestTruckAttendance>()
                .ForMember(dest => dest.AttendanceDate, opt=> opt.Ignore());

            CreateMap<DedicatedShippingRequestTruckAttendance, CreateOrEditTruckAttendanceDto>();

            CreateMap<DedicatedShippingRequestTruckAttendance, TruckAttendanceDto>()
                .ForMember(dst => dst.AttendanceStatusTitle, opt => opt.MapFrom(src => src.AttendaceStatus.GetEnumDescription()))
                .ForMember(dst => dst.AttendanceDate, opt => opt.MapFrom(src => src.AttendanceDate.Date));

            
             CreateMap<DedicatedShippingRequestTruck, GetDedicatedTruckForReplaceDto>()
                 .ForMember(dest => dest.TruckName, opt => opt.MapFrom(src => src.Truck.GetDisplayName()))
                .ForMember(dest => dest.OriginalTruckName, opt => opt.MapFrom(src => src.OriginalTruck !=null ? src.OriginalTruck.Truck.GetDisplayName() :""));


            CreateMap<ReplaceTruckDto, DedicatedShippingRequestTruck>();
            
                CreateMap<DedicatedShippingRequestDriver, GetDedicatedDriverForReplaceDto>()
                 .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => $"{src.DriverUser.Name} {src.DriverUser.Surname}"))
                .ForMember(dest => dest.OriginalDriverName, opt => opt.MapFrom(src => src.OriginalDriver != null ? $"{src.DriverUser.Name} {src.DriverUser.Surname}" : ""));

            
            CreateMap<ReplaceDriverDto, DedicatedShippingRequestDriver>();


        }

    }
}
