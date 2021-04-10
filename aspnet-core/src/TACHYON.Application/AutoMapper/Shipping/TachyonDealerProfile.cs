using AutoMapper;
using System;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos;
using TACHYON.TachyonPriceOffers;

namespace TACHYON.AutoMapper.Shipping
{
    public class TachyonDealerProfile : Profile
    {
        public TachyonDealerProfile()
        {
            CreateMap<ShippingRequestsCarrierDirectPricing, ShippingRequestsCarrierDirectPricingListDto>()
                .ForMember(dst => dst.CarrierName, opt => opt.MapFrom(src => src.Carrirer.Name))
                .ForMember(dst => dst.ShipperName, opt => opt.MapFrom(src => src.Request.Tenant.Name))
                .ForMember(dst => dst.StatusTitle, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestsCarrierDirectPricingStatus),src.Status)));

               CreateMap<ShippingRequestAmount, ShippingRequestAmountDto>();

            CreateMap<ShippingRequestsCarrierDirectPricing, ShippingRequestAmountDto>()
                .ForMember(dst => dst.DirectRequestId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.ClientName, opt => opt.MapFrom(src => src.Carrirer.Name))
                .ForMember(dst => dst.CarrierPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dst => dst.ShippingRequestId, opt => opt.MapFrom(src => src.RequestId))
                .ReverseMap();


            CreateMap<ShippingRequestBid, ShippingRequestAmountDto>()
            .ForMember(dst => dst.shippingRequestBidId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.ClientName, opt => opt.MapFrom(src => src.Tenant.Name))
            .ForMember(dst => dst.CarrirerTenantId, opt => opt.MapFrom(src => src.TenantId))
            .ForMember(dst => dst.CarrierPrice, opt => opt.MapFrom(src => src.BasePrice))
            .ForMember(dst => dst.ShippingRequestId, opt => opt.MapFrom(src => src.ShippingRequestId))
            .ReverseMap();

        }
    }
}
