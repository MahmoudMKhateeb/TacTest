using AutoMapper;
using System;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.TachyonPriceOffers;
using TACHYON.TachyonPriceOffers.dtos;

namespace TACHYON.AutoMapper.TachyonPriceOffers
{
    class TachyonPriceOfferProfile : Profile
    {
        public TachyonPriceOfferProfile()
        {
            CreateMap<TachyonPriceOffer, TachyonPriceOfferDto>()
                .ForMember(dest => dest.OfferedPrice,
                    opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.PriceTypeName,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(PriceType), src.PriceType)))
                .ForMember(dest => dest.OfferStatusName,
                    opt => opt.MapFrom(src => Enum.GetName(typeof(OfferStatus), src.OfferStatus)));


            CreateMap<CreateOrEditTachyonPriceOfferDto, TachyonPriceOffer>()
                .ReverseMap();

            CreateMap<TachyonPriceOffer, ShippingRequestAmountDto>()
                .ForMember(dst => dst.OfferId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsGuesingPrice,
                    opt => opt.MapFrom(src => src.OfferStatus == OfferStatus.AcceptedAndWaitingForCarrier))
                .ReverseMap();
            //CreateMap<TachyonPriceOffer, ShippingRequest>().ForMember(dest=>dest.Id,opt=>opt.MapFrom(src=>src.Id))
            // .ForMember(dest => dest.CarrierTenantId, opt => opt.MapFrom(src => src.CarrirerTenantId))
            // .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.TotalAmount));
        }
    }
}