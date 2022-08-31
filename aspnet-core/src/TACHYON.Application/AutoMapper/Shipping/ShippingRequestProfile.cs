using AutoMapper;
using System;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestProfile : Profile
    {
        public ShippingRequestProfile()
        {
            CreateMap<ShippingRequest, ShippingRequestListDto>()
                .ForMember(dst => dst.Tenant, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                //.ForMember(dst => dst.Destination, opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                .ForMember(dst => dst.RouteType,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestRouteType), src.RouteTypeId)));

            CreateMap<ShippingRequest, GetShippingRequestForViewOutput>()
                .ForMember(dest => dest.TruckTypeId, opt => opt.MapFrom(x => x.TrucksTypeId))
                .ForMember(dest => dest.ShippingRequest, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ShippingRequestBidDtoList, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedTruckDto, opt => opt.MapFrom(src => src.AssignedTruckFk))
                .ForMember(dest => dest.VasCount, opt => opt.MapFrom(src => src.ShippingRequestVases.Count))
                .ForMember(dest => dest.OriginalCityName, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                .ForMember(dest => dest.OriginalCityId, opt => opt.MapFrom(src => src.OriginCityFk.Id))
                //.ForMember(dest => dest.DestinationCityName,
                //    opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                .ForMember(dest => dest.DestinationCitiesDtos, opt => opt.MapFrom(src => src.ShippingRequestDestinationCities))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.AssignedDriverUserFk.Name))
                //.ForMember(dest => dest.GoodsCategoryName, opt => opt.MapFrom(src => src.GoodCategoryFk.DisplayName))
                .ForMember(dest => dest.RoutTypeName,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestRouteType), src.RouteTypeId)))
                .ForMember(dest => dest.ShippingRequestStatusName,
                    opt => opt.MapFrom(src => src.Status.GetEnumDescription()))
                //.ForMember(dest => dest.TruckTypeDisplayName, opt => opt.MapFrom(src => src.TrucksTypeFk.DisplayName))
                .ForMember(dest => dest.TruckTypeFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CapacityDisplayName, opt => opt.MapFrom(src => src.CapacityFk.DisplayName))
                .ForMember(dest => dest.TransportTypeDisplayName,
                    opt => opt.MapFrom(src => src.TransportTypeFk.DisplayName))
                .ForMember(dest => dest.ShippingTypeDisplayName,
                    opt => opt.MapFrom(src => src.ShippingTypeFk.DisplayName))
                .ForMember(dest => dest.packingTypeDisplayName,
                    opt => opt.MapFrom(src => src.PackingTypeFk.DisplayName))
                .ForMember(dest => dest.CarrierName, opt => opt.MapFrom(src => src.CarrierTenantFk.Name));
            //.AfterMap(AssignTruckTypeFullName);

            CreateMap<ShippingRequest, GetShippingRequestForPricingOutput>()
                .ForMember(dst => dst.Shipper, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dst => dst.OriginCity, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                //.ForMember(dst => dst.DestinationCity, opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                .ForMember(dst => dst.RangeDate,
                    opt => opt.MapFrom(src => GetDateRange(src.StartTripDate, src.EndTripDate)));


            CreateMap<CreateOrEditShippingRequestStep1Dto, ShippingRequest>()
                .ForMember(dest => dest.IsDrafted, opt => opt.Ignore())
                .ForMember(dest => dest.DraftStep, opt => opt.Ignore());

            CreateMap<ShippingRequest, CreateOrEditShippingRequestStep1Dto>();

            //EditShippingRequestStep2Dto moved to customDtoMapper due to ShippingRequestCityList

            CreateMap<EditShippingRequestStep3Dto, ShippingRequest>()
                .ForMember(dest => dest.IsDrafted, opt => opt.Ignore())
                .ForMember(dest => dest.DraftStep, opt => opt.Ignore());

            CreateMap<ShippingRequest, EditShippingRequestStep3Dto>();

            CreateMap<ShippingRequestDestinationCity, ShippingRequestDestinationCitiesDto>()
                .ForMember(dst => dst.CityName, opt => opt.MapFrom(src => src.CityFk.DisplayName))
                .ReverseMap();
            //EditShippingRequestStep4Dto in CustomDtoMapper
        }

        private string GetDateRange(DateTime? StartTripDate, DateTime? EndTripDate)
        {
            if (StartTripDate.HasValue && EndTripDate.HasValue)
            {
                return string.Format("{0}-{1}", StartTripDate.Value.ToString("dd/MM/yyyy"),
                    EndTripDate.Value.ToString("dd/MM/yyyy"));
            }
            else if (StartTripDate.HasValue) return StartTripDate.Value.ToString("dd/MM/yyyy");

            return "";
        }
        //private static void AssignTruckTypeFullName(ShippingRequest shippingRequest, GetShippingRequestForViewOutput dto)
        //{
        //    dto.TruckTypeFullName = shippingRequest.TransportTypeFk?.DisplayName
        //                            + "-" + shippingRequest.TrucksTypeFk?.DisplayName
        //                            + "-" + shippingRequest?.CapacityFk?.DisplayName;
        //}
    }
}