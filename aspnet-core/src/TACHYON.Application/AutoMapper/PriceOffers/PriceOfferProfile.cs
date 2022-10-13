using Abp.Timing;
using AutoMapper;
using System;
using System.Linq;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.ShippingRequestVases;

namespace TACHYON.AutoMapper.PriceOffers
{
    public class PriceOfferProfile : Profile
    {
        public PriceOfferProfile()
        {
            CreateMap<CreateOrEditPriceOfferInput, PriceOffer>().ReverseMap();
            CreateMap<PriceOfferDetail, PriceOfferItem>();
            CreateMap<PriceOffer, PriceOfferListDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dto => dto.CarrierTenantId, dto => dto.MapFrom(src => src.TenantId))
                .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => src.Status.GetEnumDescription()));


            CreateMap<PriceOffer, PriceOfferDto>()
                .ForMember(dst => dst.Items, opt => opt.MapFrom(src => src.PriceOfferDetails));
            CreateMap<PriceOffer, PriceOfferViewDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dst => dst.EditionId, opt => opt.MapFrom(src => src.Tenant.EditionId))
                .ForMember(dst => dst.ShippingRequestStatus, opt => opt.MapFrom(src => src.ShippingRequestFk.Status))
                .ForMember(dst => dst.IsTachyonDeal, opt => opt.MapFrom(src => src.ShippingRequestFk.IsTachyonDeal))
                .ForMember(dst => dst.Items, opt => opt.MapFrom(src => src.PriceOfferDetails));

            CreateMap<ShippingRequestVas, PriceOfferItemDto>()
                .ForMember(dst => dst.ItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.ParentItemId, opt => opt.MapFrom(src => src.VasId))
                .ForMember(dst => dst.ItemName, opt => opt.MapFrom(src => src.VasFk.Key))
                .ForMember(dst => dst.PriceType, opt => opt.MapFrom(src => PriceOfferType.Vas))
                .ForMember(dst => dst.Quantity, opt => opt.MapFrom(src => src.RequestMaxCount >= 1 ? src.RequestMaxCount : 1))
                .ForMember(dst => dst.IsAppearAmount, opt => opt.MapFrom(src => src.VasFk.IsAppearAmount))
                .ForMember(dst => dst.Amount, opt => opt.MapFrom(src => src.RequestMaxAmount >= 1 ? src.RequestMaxAmount : 1));

            CreateMap<ShippingRequest, GetShippingRequestForPriceOfferListDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dst => dst.ShipperRating, opt => opt.MapFrom(src => src.Tenant.Rate))
                .ForMember(dst => dst.ShipperRatingNumber, opt => opt.MapFrom(src => src.Tenant.RateNumber))
                .ForMember(dst => dst.Carrier, opt => opt.MapFrom(src => src.CarrierTenantFk.Name))
                .ForMember(dst => dst.ShipperName, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dst => dst.OriginCity, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                .ForMember(dst => dst.destinationCities, opt => opt.MapFrom(src => src.ShippingRequestDestinationCities))
                //.ForMember(dst => dst.DestinationCity, opt => opt.MapFrom(src => src.ShippingRequestDestinationCities.First().CityFk.DisplayName))
                .ForMember(dst => dst.BidStatusTitle, opt => opt.MapFrom(src => src.BidStatus.GetEnumDescription()))
                .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => src.Status.GetEnumDescription()))
                .ForMember(dst => dst.RemainingDays, opt => opt.MapFrom(src => "0"))
                .ForMember(dst => dst.RemainingDays,
                    opt => opt.MapFrom(src => GetRemainingDays(src.BidEndDate, src.BidStatus)))
                .ForMember(dst => dst.Price, opt => opt.MapFrom(src => src.Price))
                 .ForMember(dst => dst.ShippingRequestFlagTitle,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestFlag), src.ShippingRequestFlag)))
                .ForMember(dst => dst.RentalDurationUnitTitle,
                    opt => opt.MapFrom(src =>GetDurationUnit(src.RentalDurationUnit.Value)));
        }

        private string GetRemainingDays(DateTime? BidEndDate, ShippingRequestBidStatus Status)
        {
            if (BidEndDate.HasValue && Status != ShippingRequestBidStatus.OnGoing)
            {
                int TotalDays = (int)((BidEndDate.Value - Clock.Now).TotalDays);
                if (TotalDays <= 0) return "0";
                if (TotalDays < 9) return $"0{TotalDays}";
                return TotalDays.ToString();
            }

            return "0";
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

        private static string GetDurationUnit(TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Daily:
                    return "Days";
                case TimeUnit.Monthly:
                    return "Months";
                case TimeUnit.Weekly:
                    return "Weeks";
                default:
                    return "";
            }
        }
    }
}