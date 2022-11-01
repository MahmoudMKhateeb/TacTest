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


        }

    }
}
