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

            CreateMap<ShippingRequest, GetShippingRequestForViewOutput>()
                .ForMember(dest => dest.ShippingRequest, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ShippingRequestBidDtoList, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedTruckDto, opt => opt.MapFrom(src => src.AssignedTruckFk))
                .ForMember(dest => dest.VasCount, opt => opt.MapFrom(src => src.ShippingRequestVases.Count))
                .ForMember(dest => dest.OriginalCityName, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                .ForMember(dest => dest.DestinationCityName, opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.AssignedDriverUserFk.Name))
                //.ForMember(dest => dest.GoodsCategoryName, opt => opt.MapFrom(src => src.GoodCategoryFk.DisplayName))

                .ForMember(dest => dest.RoutTypeName, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestRouteType), src.RouteTypeId)))
                .ForMember(dest => dest.ShippingRequestStatusName, opt => opt.MapFrom(src => src.Status.GetEnumDescription()))
                .ForMember(dest => dest.TruckTypeDisplayName, opt => opt.MapFrom(src => src.TrucksTypeFk.DisplayName))
                .ForMember(dest => dest.TruckTypeFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CapacityDisplayName, opt => opt.MapFrom(src => src.CapacityFk.DisplayName))
                .ForMember(dest => dest.TransportTypeDisplayName, opt => opt.MapFrom(src => src.TransportTypeFk.DisplayName))
                .ForMember(dest => dest.ShippingTypeDisplayName, opt => opt.MapFrom(src => src.ShippingTypeFk.DisplayName))
                .ForMember(dest => dest.packingTypeDisplayName, opt => opt.MapFrom(src => src.PackingTypeFk.DisplayName))
                .ForMember(dest => dest.CarrierName, opt => opt.MapFrom(src => src.CarrierTenantFk.Name))
                .AfterMap(AssignTruckTypeFullName);

        }

        private static void AssignTruckTypeFullName(ShippingRequest shippingRequest, GetShippingRequestForViewOutput dto)
        {
            dto.TruckTypeFullName = shippingRequest.TransportTypeFk?.DisplayName
                                    + "-" + shippingRequest.TrucksTypeFk?.DisplayName
                                    + "-" + shippingRequest?.CapacityFk?.DisplayName;
        }
    }
}
