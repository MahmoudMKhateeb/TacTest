using AutoMapper;
using TACHYON.MarketPlaces.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.AutoMapper.MarketPlaces
{
    public class MarketPlaceProfile: Profile
    {
        public MarketPlaceProfile()
        {
            CreateMap<ShippingRequest, MarketPlaceListDto>()
                 .ForMember(dst => dst.Shipper, opt => opt.MapFrom(src => src.Tenant.companyName))
                 .ForMember(dst => dst.OriginCity, opt => opt.MapFrom(src => src.OriginCityFk.DisplayName))
                 .ForMember(dst => dst.DestinationCity, opt => opt.MapFrom(src => src.DestinationCityFk.DisplayName))
                 ;
        }
    }
}
