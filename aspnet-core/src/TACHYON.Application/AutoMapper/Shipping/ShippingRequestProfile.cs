using AutoMapper;
using System;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestProfile : Profile
    {
        public ShippingRequestProfile ()
        {
            CreateMap<ShippingRequest, ShippingRequestListDto>()
                .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                .ForMember(dst => dst.Destination, opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                .ForMember(dst => dst.RouteType, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestRouteType),src.RouteTypeId)));

        }
    }
}
