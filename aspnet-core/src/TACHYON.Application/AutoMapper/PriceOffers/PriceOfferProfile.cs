using AutoMapper;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.AutoMapper.PriceOffers
{
    public class PriceOfferProfile: Profile
    {
        public PriceOfferProfile()
        {

            CreateMap<CreateOrEditPriceOfferInput, PriceOffer>();
            CreateMap<PriceOffer, ShippingRequestCarrierPricingDto>();

        }
    }
}
