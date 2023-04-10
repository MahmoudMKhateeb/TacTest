using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Common;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Tracking.AdditionalSteps;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.AutoMapper.Tracking
{
    public class TrackingProfile : Profile
    {
        public TrackingProfile()
        {
            CreateMap<ShippingRequestTrip, TrackingListDto>()
             .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ShippingRequestFk.CarrierTenantFk.Name))
             .ForMember(dst => dst.RouteTypeId, opt => opt.MapFrom(src => src.RouteType != null ?src.RouteType :src.ShippingRequestFk.RouteTypeId))
            .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk.FullName))
            .ForMember(dst => dst.DriverImageProfile, opt => opt.MapFrom(src => src.AssignedDriverUserFk.ProfilePictureId))
            .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.OriginFacilityFk.Address))
            .ForMember(dst => dst.Destination, opt => opt.MapFrom(src => src.DestinationFacilityFk.Address))
            .ForMember(dst => dst.ReferenceNumber, opt => opt.MapFrom(src => src.ShippingRequestFk.ReferenceNumber))
            .ForMember(dst => dst.ShippingRequestFlag, opt => opt.MapFrom(src => src.ShippingRequestFk.ShippingRequestFlag))
            .ForMember(dst => dst.NumberOfTrucks, opt => opt.MapFrom(src => src.ShippingRequestFk.NumberOfTrucks))
            .ForMember(dst => dst.TenantId, opt => opt.MapFrom(src => src.ShippingRequestFk.TenantId))
            .ForMember(dst => dst.ShippingType, opt => opt.MapFrom(src => src.ShippingRequestFk.ShippingTypeId))
            .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src.ShippingRequestId))
            .ForMember(dst => dst.DriverRate, opt => opt.MapFrom(src => src.AssignedDriverUserFk.Rate))
            .ForMember(dst => dst.shippingRequestStatus, opt => opt.MapFrom(src => src.ShippingRequestFk.Status))
            .ForMember(dst => dst.IsPrePayedShippingRequest, opt => opt.MapFrom(src => src.ShippingRequestFk.IsPrePayed))
            .ForMember(dst => dst.TripFlag, opt => opt.MapFrom(src => src.ShippingRequestTripFlag))
            .ForMember(dst => dst.IsSass, opt => opt.MapFrom(src => src.ShippingRequestFk.IsSaas()))
            .ForMember(dst => dst.ShippingRequestFlagTitle, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.ShippingRequestFlag.GetEnumDescription() :"SAAS"))
            //todo this should be updated after release apply shipping type in SAAS shipment
            .ForMember(dst => dst.ShippingTypeTitle, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.ShippingTypeId.GetEnumDescription() :""))
            .ForMember(dst => dst.PlateNumber, opt => opt.MapFrom(src => src.AssignedTruckFk != null ?src.AssignedTruckFk.PlateNumber :""));


            CreateMap<IHasDocument, RoutPointDocument>().ReverseMap();
            CreateMap<RoutPointDocument, TripManifestDataDto>().ReverseMap();
            CreateMap<IHasDocument, AdditionalStepArgs>().ReverseMap();
            CreateMap<AdditionalStepTransition, AdditionalStepTransitionDto>()
            .ForMember(dst => dst.AdditionalStepTypeTitle, opt => opt.MapFrom(src => src.AdditionalStepType.GetEnumDescription()))
            .ForMember(dst => dst.RoutePointDocumentTypeTitle, opt => opt.MapFrom(src => src.RoutePointDocumentType.GetEnumDescription()));
        }
    }
}