using Abp.Timing;
using AutoMapper;
using System;
using System.Linq;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.ShippingRequestTripVases;
using TACHYON.Tracking;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class ShippingRequestTripProfile : Profile
    {
        public ShippingRequestTripProfile()
        {
            CreateMap<ShippingRequestTrip, ShippingRequestsTripListDto>()
                .ForMember(dst => dst.OriginCity,
                    opt => opt.MapFrom(src =>
                        src.OriginFacilityFk != null ? src.OriginFacilityFk.CityFk.DisplayName : ""))
                .ForMember(dst => dst.DestinationCity,
                    opt => opt.MapFrom(src =>
                        src.DestinationFacilityFk != null ? src.DestinationFacilityFk.CityFk.DisplayName : ""))
                .ForMember(dst => dst.Truck,
                    opt => opt.MapFrom(
                        src => src.AssignedTruckFk != null ? src.AssignedTruckFk.ModelName : string.Empty))
                .ForMember(dst => dst.Driver,
                    opt => opt.MapFrom(src =>
                        src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Name : string.Empty))
                .ForMember(dst => dst.DriverStatusTitle, opt => opt.MapFrom(src =>
                    src.DriverStatus == ShippingRequestTripDriverStatus.None
                    && src.AssignedDriverUserFk != null
                        ? "NeedAccept"
                        : Enum.GetName(typeof(ShippingRequestTripDriverStatus), src.DriverStatus)));

            CreateMap<ShippingRequestTrip, ShippingRequestsTripForViewDto>()
               .ForMember(dst => dst.OriginFacility, opt => opt.MapFrom(src => src.OriginFacilityFk != null ? $"{src.OriginFacilityFk.Name} - {src.OriginFacilityFk.Address}" : ""))
               .ForMember(dst => dst.DestinationFacility, opt => opt.MapFrom(src => src.DestinationFacilityFk != null ? $"{src.DestinationFacilityFk.Name} - {src.DestinationFacilityFk.Address}" : ""))
               .ForMember(dst => dst.Truck, opt => opt.MapFrom(src => src.AssignedTruckFk != null ? src.AssignedTruckFk.ModelName : string.Empty))
               .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Name : string.Empty))
               .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
               .ForMember(dst => dst.RoutePointStatus, opt => opt.MapFrom(src => Enum.GetName(typeof(RoutePointStatus), src.Status)))
               .ForMember(dst => dst.DriverStatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripDriverStatus), src.DriverStatus)))
               .ForMember(dst => dst.RoutPoints, opt => opt.MapFrom(src => src.RoutPoints))
               .ForMember(dst => dst.ShippingRequestTripVases, opt => opt.MapFrom(src => src.ShippingRequestTripVases))
               .ForMember(dst => dst.TotalValue, opt => opt.MapFrom(src => src.TotalValue));

            CreateMap<ShippingRequestTrip, ShippingRequestTripDriverListDto>()
                .ForMember(dst => dst.Source,
                    opt => opt.MapFrom(src =>
                        $"{src.ShippingRequestFk.OriginCityFk.DisplayName} - {src.OriginFacilityFk.Address}"))
                .ForMember(dst => dst.Distination,
                    opt => opt.MapFrom(src =>
                        src.DestinationFacilityFk.Address))
                .ForMember(dst => dst.RouteTypeId, opt => opt.MapFrom(src => src.ShippingRequestFk.RouteTypeId))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTripDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndTripDate))
                .ForMember(dst => dst.IsSaas, opt => opt.MapFrom(src => src.ShippingRequestFk.IsSaas()))
                .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(RoutePointStatus), src.RoutePointStatus)))
                .ForMember(dst => dst.TripStatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
                .ForMember(dst => dst.DriverLoadStatus, opt => opt.MapFrom(src => GetMobileTripStatus(src)));


            CreateMap<ShippingRequestTrip, ShippingRequestTripDriverDetailsDto>()
                .ForMember(dst => dst.ShipperName, opt => opt.MapFrom(src => src.ShippingRequestFk.Tenant.TenancyName))
                .ForMember(dst => dst.ShipperRating, opt => opt.MapFrom(src => src.ShippingRequestFk.Tenant.Rate))
                .ForMember(dst => dst.Source,
                    opt => opt.MapFrom(src =>
                        $"{src.ShippingRequestFk.OriginCityFk.DisplayName} - {src.OriginFacilityFk.Address}"))
                .ForMember(dst => dst.SourceFacilityRating, opt => opt.MapFrom(src => src.OriginFacilityFk.Rate))
                .ForMember(dst => dst.SourceFacilityRatingNumber,
                    opt => opt.MapFrom(src => src.OriginFacilityFk.RateNumber))
                .ForMember(dst => dst.Distination,
                    opt => opt.MapFrom(src =>
                        src.DestinationFacilityFk.Address))
                .ForMember(dst => dst.DestinationFacilityRating,
                    opt => opt.MapFrom(src => src.DestinationFacilityFk.Rate))
                .ForMember(dst => dst.DestinationFacilityRatingNumber,
                    opt => opt.MapFrom(src => src.DestinationFacilityFk.RateNumber))
                .ForMember(dst => dst.StartTripDate,
                    opt => opt.MapFrom(
                        src => src.StartTripDate == null ? "" : src.StartTripDate.ToString("dd,MMM,yyyy")))
                .ForMember(dst => dst.EndTripDate,
                    opt => opt.MapFrom(src =>
                        src.EndTripDate == null ? "" :
                        src.EndTripDate != null ? src.EndTripDate.Value.ToString("dd,MMM,yyyy") : ""))
                .ForMember(dst => dst.TravelTime,
                    opt => opt.MapFrom(src =>
                        src.StartWorking == null ? "" : ((DateTime)src.StartWorking).ToString("hh:mm")))
                .ForMember(dst => dst.TotalWeight, opt => opt.MapFrom(src => src.ShippingRequestFk.TotalWeight))
                .ForMember(dst => dst.PackingType,
                    opt => opt.MapFrom(src => src.ShippingRequestFk.PackingTypeFk.DisplayName))
                .ForMember(dst => dst.PlateNumber, opt => opt.MapFrom(src => src.AssignedTruckFk.PlateNumber))
                .ForMember(dst => dst.RoutePoints,
                    opt => opt.MapFrom(src => src.RoutPoints.OrderBy(x => x.PickingType)))
                .ForMember(dst => dst.StatusTitle,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(RoutePointStatus), src.RoutePointStatus)))
                .ForMember(dst => dst.TripStatus,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.RoutePointStatus));


            //GoodsDetails

            CreateMap<RoutPoint, ShippingRequestTripDriverRoutePointDto>()
                .ForMember(dst => dst.Address, opt => opt.MapFrom(src => src.FacilityFk.Address))
                .ForMember(dst => dst.Status,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(RoutePointStatus), src.Status)))
                .ForMember(dst => dst.CompletedStatus,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(RoutePointCompletedStatus), src.CompletedStatus)))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => src.FacilityId))
                .ForMember(dst => dst.Facility, opt => opt.MapFrom(src => src.FacilityFk.Name))
                .ForMember(dst => dst.FacilityRating, opt => opt.MapFrom(src => src.FacilityFk.Rate))
                .ForMember(dst => dst.FacilityRatingNumber, opt => opt.MapFrom(src => src.FacilityFk.RateNumber))
                .ForMember(dst => dst.lat, opt => opt.MapFrom(src => src.FacilityFk.Location.Y))
                .ForMember(dst => dst.lng, opt => opt.MapFrom(src => src.FacilityFk.Location.X))
                .ForMember(dst => dst.GoodsDetails, opt => opt.MapFrom(src => src.GoodsDetails))
                .ForMember(dst => dst.ReceiverFullName,
                    opt => opt.MapFrom(src =>
                        !string.IsNullOrEmpty(src.ReceiverFullName) ? src.ReceiverFullName :
                        src.ReceiverFk != null ? src.ReceiverFk.FullName : ""));

            CreateMap<ShippingRequestTrip, CreateOrEditShippingRequestTripDto>()
                .ForMember(dest => dest.RoutPoints, opt => opt.MapFrom(src => src.RoutPoints))
                .ForMember(dest => dest.ShippingRequestTripVases,
                    opt => opt.MapFrom(src => src.ShippingRequestTripVases));

            CreateMap<ShippingRequestTrip, ShippingRequestTripDto>().ReverseMap();
            
            CreateMap<ImportTripDto,ShippingRequestTrip>()
                .ReverseMap();

            CreateMap<ImportTripVasesDto, ShippingRequestTripVas>();
        }

        private static ShippingRequestTripDriverLoadStatusDto GetMobileTripStatus(ShippingRequestTrip trip)
        {
            if (trip.StartTripDate.Date <= Clock.Now.Date && trip.Status != ShippingRequestTripStatus.Delivered &&
                trip.Status != ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
                return ShippingRequestTripDriverLoadStatusDto.Current;
            if (trip.Status == ShippingRequestTripStatus.Delivered ||
                trip.Status == ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
                return ShippingRequestTripDriverLoadStatusDto.Past;
            return ShippingRequestTripDriverLoadStatusDto.Comming;
        }
    }
}