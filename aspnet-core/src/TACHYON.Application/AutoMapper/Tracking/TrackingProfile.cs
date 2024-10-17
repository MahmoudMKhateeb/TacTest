using Abp.Application.Features;
using Abp.Runtime.Session;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
             .ForMember(dst => dst.ShipperName, opt => opt.MapFrom(src => src.ShippingRequestFk!= null ? src.ShippingRequestFk.Tenant.Name :src.ShipperTenantFk.Name))
             .ForMember(dst => dst.CarrierName, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.CarrierTenantFk.Name :src.CarrierTenantFk.Name))
             .ForMember(dst => dst.ShipperActorNAme, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.ShipperActorFk.CompanyName: src.ShipperActorFk.CompanyName))
             .ForMember(dst => dst.CarrierActorNam, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.CarrierActorFk.CompanyName :src.CarrierActorFk.CompanyName))
             .ForMember(dst => dst.RouteTypeId, opt => opt.MapFrom(src => src.RouteType != null ? src.RouteType : src.ShippingRequestFk.RouteTypeId))
            .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Name+" "+ src.AssignedDriverUserFk.Surname : ""))
            .ForMember(dst => dst.DriverImageProfile, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.ProfilePictureId : null))
            .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.OriginFacilityFk != null ? src.OriginFacilityFk.Name : ""))
            .ForMember(dst => dst.Destination, opt => opt.MapFrom(src => src.DestinationFacilityFk.Name))
            .ForMember(dst => dst.ReferenceNumber, opt => opt.MapFrom(src => src.ShippingRequestId.HasValue? src.ShippingRequestFk.ShipperReference :  src.ShippingRequestFk.ReferenceNumber))
            .ForMember(dst => dst.ShippingRequestFlag, opt => opt.MapFrom(src => src.ShippingRequestFk.ShippingRequestFlag))
            .ForMember(dst => dst.NumberOfTrucks, opt => opt.MapFrom(src => src.ShippingRequestFk.NumberOfTrucks))
            .ForMember(dst => dst.TenantId, opt => opt.MapFrom(src => src.ShippingRequestFk.TenantId))
            .ForMember(dst => dst.ShippingType, opt => opt.MapFrom(src => src.ShippingRequestId.HasValue ? src.ShippingRequestFk.ShippingTypeId : src.ShippingTypeId))
            .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src.ShippingRequestId))
            .ForMember(dst => dst.DriverRate, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Rate : 0))
            .ForMember(dst => dst.shippingRequestStatus, opt => opt.MapFrom(src => src.ShippingRequestFk.Status))
            .ForMember(dst => dst.IsPrePayedShippingRequest, opt => opt.MapFrom(src => src.ShippingRequestFk.IsPrePayed))
            .ForMember(dst => dst.TripFlag, opt => opt.MapFrom(src => src.ShippingRequestTripFlag))
            .ForMember(dst => dst.IsSass, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.IsSaas() : src.ShipperTenantId == src.CarrierTenantId))
            .ForMember(dst => dst.NumberOfDrops, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.NumberOfDrops : src.NumberOfDrops))
            .ForMember(dst => dst.TruckType, opt => opt.MapFrom(src => src.ShippingRequestId.HasValue? src.ShippingRequestFk.TrucksTypeFk.DisplayName :  src.AssignedTruckFk.TrucksTypeFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
            ? src.AssignedTruckFk.TrucksTypeFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().TranslatedDisplayName
            : src.AssignedTruckFk.TrucksTypeFk.Key))
            .ForMember(dst => dst.GoodsCategory, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
            ? src.ShippingRequestFk.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().DisplayName
            : src.ShippingRequestFk.GoodCategoryFk.Key
            : src.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
            ? src.GoodCategoryFk.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().DisplayName
            : src.GoodCategoryFk.Key))
            .ForMember(dst => dst.Reason, opt => opt.MapFrom(src => src.ShippingRequestTripRejectReason != null
            ? src.ShippingRequestTripRejectReason.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault() != null
            ? src.ShippingRequestTripRejectReason.Translations.Where(x => x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault().Name
            : src.ShippingRequestTripRejectReason.Key
            : ""))
            .ForMember(dst => dst.CarrierTenantId, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.CarrierTenantId : src.CarrierTenantId))
            .ForMember(dst => dst.ShippingRequestFlagTitle, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.ShippingRequestFlag.GetEnumDescription() : "SAAS"))
            .ForMember(dst => dst.ShippingTypeTitle, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.ShippingTypeId.GetEnumDescription() : src.ShippingTypeId.GetEnumDescription()))
            .ForMember(dst => dst.PlateNumber, opt => opt.MapFrom(src => src.AssignedTruckFk != null ? src.AssignedTruckFk.PlateNumber : ""))
            .ForMember(dst => dst.DriverStatusTitle, opt => opt.MapFrom(src => src.DriverStatus.GetEnumDescription()))
            .ForMember(dst => dst.RouteType, opt => opt.MapFrom(src => src.RouteType != null ? src.RouteType.GetEnumDescription() : src.ShippingRequestFk.RouteTypeId.GetEnumDescription()))
            .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => src.Status.GetEnumDescription()))
            .ForMember(dst => dst.BookingNumber, opt => opt.MapFrom(src => src.ShippingRequestFk != null ?src.ShippingRequestFk.ShipperInvoiceNo  :src.ShipperInvoiceNo))
            .ForMember(dst => dst.ActualDeliveryDate, opt => opt.MapFrom(src => src.ActualDeliveryDate != null ?src.ActualDeliveryDate.Value.ToString("dd/MM/yyyy")  :""))
            .ForMember(dst => dst.ActualPickupDate, opt => opt.MapFrom(src => src.StartTripDate.ToString("dd/MM/yyyy")))
            .ForMember(dst => dst.IsPODUploaded, opt => opt.MapFrom(src => src.RoutPoints.Where(x=>x.PickingType == PickingType.Dropoff).All(x=>x.IsPodUploaded)))
            .ForMember(dst => dst.IsInvoiceIssued, opt => opt.MapFrom(src => src.IsShipperHaveInvoice))
            .ForMember(dst => dst.ActorShipperSubTotalAmountWithCommission, opt => opt.MapFrom(src => src.ShippingRequestFk != null ? src.ShippingRequestFk.ActorShipperPrice.SubTotalAmountWithCommission : src.ActorShipperPrice.SubTotalAmountWithCommission))
            .ForMember(dst => dst.ActorShipperTotalAmountWithCommission, opt => opt.MapFrom(src => src.ShippingRequestFk != null ?src.ShippingRequestFk.ActorShipperPrice.TotalAmountWithCommission :  src.ActorShipperPrice.TotalAmountWithCommission))
            .ForMember(dst => dst.ReplacedDriver, opt => opt.MapFrom(src => src.ReplacesDriverFk != null ? src.ReplacesDriverFk.Name+" "+ src.ReplacesDriverFk.Surname : ""))
            ;


            CreateMap<IHasDocument, RoutPointDocument>().ReverseMap();
            CreateMap<RoutPointDocument, TripManifestDataDto>().ReverseMap();
            CreateMap<IHasDocument, AdditionalStepArgs>().ReverseMap();
            CreateMap<AdditionalStepTransition, AdditionalStepTransitionDto>()
            .ForMember(dst => dst.AdditionalStepTypeTitle, opt => opt.MapFrom(src => src.AdditionalStepType.GetEnumDescription()))
            .ForMember(dst => dst.RoutePointDocumentTypeTitle, opt => opt.MapFrom(src => src.RoutePointDocumentType.GetEnumDescription()));
        }
    }
}