using AutoMapper;
using System;
using TACHYON.Common;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class ShippingRequestTripAccidenCommentProfile : Profile
    {
        public ShippingRequestTripAccidenCommentProfile()
        {
            CreateMap<ShippingRequestTripAccidentComment, ShippingRequestTripAccidentCommentListDto>()
                .ForMember(dst => dst.TenantName,
                    opt => opt.MapFrom(src => src.TenantFK != null ? src.TenantFK.Name : "")).ReverseMap();

            CreateMap<ShippingRequestTripAccidentComment, CreateOrEditShippingRequestTripAccidentCommentDto>()
                .ReverseMap();
        }
    }
}