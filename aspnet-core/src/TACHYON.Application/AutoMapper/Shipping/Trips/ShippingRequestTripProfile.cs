using Abp.Timing;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class ShippingRequestTripProfile : Profile
    {

        public ShippingRequestTripProfile()
        {
            CreateMap<ShippingRequestTrip, ShippingRequestsTripListDto>()
                 .ForMember(dst => dst.OriginFacility, opt => opt.MapFrom(src => src.OriginFacilityFk != null ? $"{src.OriginFacilityFk.Name} - {src.OriginFacilityFk.Address}" : ""))
                 .ForMember(dst => dst.DestinationFacility, opt => opt.MapFrom(src => src.DestinationFacilityFk != null ? $"{src.DestinationFacilityFk.Name} - {src.DestinationFacilityFk.Address}" : ""))
                 .ForMember(dst => dst.Truck, opt => opt.MapFrom(src => src.AssignedTruckFk != null ? src.AssignedTruckFk.ModelName : string.Empty))
                 .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Name : string.Empty))
                 .ForMember(dst => dst.DriverStatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripDriverStatus), src.DriverStatus)))
                 .ForMember(dst => dst.RejectedReason, opt => opt.MapFrom(src => src.ShippingRequestTripRejectReason != null ? src.ShippingRequestTripRejectReason.DisplayName : src.RejectedReason));

            CreateMap<ShippingRequestTrip, ShippingRequestsTripForViewDto>()
               .ForMember(dst => dst.OriginFacility, opt => opt.MapFrom(src => src.OriginFacilityFk != null ? $"{src.OriginFacilityFk.Name} - {src.OriginFacilityFk.Address}" : ""))
               .ForMember(dst => dst.DestinationFacility, opt => opt.MapFrom(src => src.DestinationFacilityFk != null ? $"{src.DestinationFacilityFk.Name} - {src.DestinationFacilityFk.Address}" : ""))
               .ForMember(dst => dst.Truck, opt => opt.MapFrom(src => src.AssignedTruckFk != null ? src.AssignedTruckFk.ModelName : string.Empty))
               .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Name : string.Empty))
               .ForMember(dst => dst.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
               .ForMember(dst => dst.DriverStatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripDriverStatus), src.DriverStatus)))
               .ForMember(dst => dst.RejectedReason, opt => opt.MapFrom(src => src.ShippingRequestTripRejectReason != null ? src.ShippingRequestTripRejectReason.DisplayName : src.RejectedReason))
               .ForMember(dst => dst.RoutPoints, opt => opt.MapFrom(src => src.RoutPoints))
               .ForMember(dst => dst.ShippingRequestTripVases, opt => opt.MapFrom(src => src.ShippingRequestTripVases))
               .ForMember(dst => dst.TotalValue, opt => opt.MapFrom(src => src.TotalValue));

            CreateMap<ShippingRequestTrip, ShippingRequestTripDriverListDto>()
                .ForMember(dst => dst.Source, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.OriginCityFk.DisplayName} - {src.OriginFacilityFk.Address}"))
                .ForMember(dst => dst.Distination, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.DestinationCityFk.DisplayName} - {src.DestinationFacilityFk.Address}"))
                .ForMember(dst => dst.RouteTypeId, opt => opt.MapFrom(src => src.ShippingRequestFk.RouteTypeId))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTripDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndTripDate))
                .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
                .ForMember(dst => dst.DriverLoadStatus, opt => opt.MapFrom(src => GetMobileTripStatus(src)));



            CreateMap<ShippingRequestTrip, ShippingRequestTripDriverDetailsDto>()
                .ForMember(dst => dst.Source, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.OriginCityFk.DisplayName} - {src.OriginFacilityFk.Address}"))
                .ForMember(dst => dst.Distination, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.DestinationCityFk.DisplayName} - {src.DestinationFacilityFk.Address}"))
                .ForMember(dst => dst.StartTripDate, opt => opt.MapFrom(src => src.StartTripDate == null ? "" : src.StartTripDate.ToString("dd,MMM,yyyy")))
                .ForMember(dst => dst.EndTripDate, opt => opt.MapFrom(src => src.EndTripDate == null ? "" : src.EndTripDate.ToString("dd,MMM,yyyy")))
                .ForMember(dst => dst.TravelTime, opt => opt.MapFrom(src => src.StartWorking == null ? "" : ((DateTime)src.StartWorking).ToString("hh:mm")))
                .ForMember(dst => dst.TotalWeight, opt => opt.MapFrom(src => src.ShippingRequestFk.TotalWeight))
                .ForMember(dst => dst.PackingType, opt => opt.MapFrom(src => src.ShippingRequestFk.PackingTypeFk.DisplayName))
                .ForMember(dst => dst.GoodsCategory, opt => opt.MapFrom(src => src.ShippingRequestFk.GoodCategoryFk.DisplayName))
                .ForMember(dst => dst.RoutePoints, opt => opt.MapFrom(src => src.RoutPoints.OrderBy(x => x.PickingType)))
                .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
                .ForMember(dst=>dst.ChangeStatusButtonTitle, opt => opt.MapFrom(src => GetMobileTripChangeStatusButtonTitle(src.Status)));





            CreateMap<RoutPoint, ShippingRequestTripDriverRoutePointDto>()
                .ForMember(dst => dst.Address, opt => opt.MapFrom(src => $"{src.FacilityFk.CityFk.DisplayName} - {src.FacilityFk.Address}"))
                .ForMember(dst => dst.Facility, opt => opt.MapFrom(src => src.FacilityFk.Name))
                .ForMember(dst => dst.lat, opt => opt.MapFrom(src => src.FacilityFk.Location.X))
                .ForMember(dst => dst.lng, opt => opt.MapFrom(src => src.FacilityFk.Location.Y));

            CreateMap<ShippingRequestTrip, CreateOrEditShippingRequestTripDto>()
                .ForMember(dest => dest.RoutPoints, opt => opt.MapFrom(src => src.RoutPoints))
                .ForMember(dest => dest.ShippingRequestTripVases, opt => opt.MapFrom(src => src.ShippingRequestTripVases));
        }

        private static ShippingRequestTripDriverLoadStatusDto GetMobileTripStatus(ShippingRequestTrip trip)
        {
            if (trip.StartTripDate.Date <= Clock.Now.Date && trip.Status != ShippingRequestTripStatus.Finished)
                return ShippingRequestTripDriverLoadStatusDto.Current;
            if (trip.Status == ShippingRequestTripStatus.Finished)
                return ShippingRequestTripDriverLoadStatusDto.Past;
            return ShippingRequestTripDriverLoadStatusDto.Comming;
        }


        private static string GetMobileTripChangeStatusButtonTitle(ShippingRequestTripStatus Status)
        {
            switch (Status)
            {
                case ShippingRequestTripStatus.StandBy:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.StartedMovingToLoadingLocation);
                case ShippingRequestTripStatus.StartedMovingToLoadingLocation:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.ArriveToLoadingLocation);
                case ShippingRequestTripStatus.ArriveToLoadingLocation:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.StartLoading);
                case ShippingRequestTripStatus.StartLoading:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.FinishLoading);
                case ShippingRequestTripStatus.StartedMovingToOfLoadingLocation:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.ArrivedToDestination);
                case ShippingRequestTripStatus.ArrivedToDestination:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.StartOffloading);
                case ShippingRequestTripStatus.StartOffloading:
                    return Enum.GetName(typeof(ShippingRequestTripStatus), ShippingRequestTripStatus.FinishOffLoadShipment);
                default:
                    return "";
            }

        }
    }

}
