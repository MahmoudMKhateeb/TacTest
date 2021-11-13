using AutoMapper;
using System;
using TACHYON.Common;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class ShippingRequestTripAccidenProfile : Profile
    {
        public ShippingRequestTripAccidenProfile()
        {
            CreateMap<ShippingRequestTripAccident, ShippingRequestTripAccidentListDto>()
            .ForMember(dst => dst.Reason, opt => opt.MapFrom(src => src.OtherReasonName))
            .ForMember(dst => dst.Address, opt => opt.MapFrom(src => $"{src.RoutPointFK.FacilityFk.CityFk.DisplayName}-{src.RoutPointFK.FacilityFk.Address}"))
            .ForMember(dst => dst.PickingType, opt => opt.MapFrom(src => Enum.GetName(typeof(PickingType), src.RoutPointFK.PickingType)));
            CreateMap<ShippingRequestTripAccident, CreateOrEditShippingRequestTripAccidentDto>()
            .ForMember(dst => dst.lat, opt => opt.MapFrom(src => src.Location != null ? src.Location.X : default(double?)))
            .ForMember(dst => dst.lng, opt => opt.MapFrom(src => src.Location != null ? src.Location.Y : default(double?)));
            CreateMap<CreateOrEditShippingRequestTripAccidentDto, ShippingRequestTripAccident>();

            CreateMap<CreateOrEditShippingRequestTripAccidentDto, DocumentUpload>().ReverseMap();


            CreateMap<CreateOrEditShippingRequestTripAccidentResolveDto, DocumentUpload>().ReverseMap();
            CreateMap<CreateOrEditShippingRequestTripAccidentResolveDto, ShippingRequestTripAccidentResolve>().ReverseMap();



        }
    }
}