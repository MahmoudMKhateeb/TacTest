using AutoMapper;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestResoneAccidentProfile: Profile
    {
        public ShippingRequestResoneAccidentProfile ()
        {
            CreateMap<ShippingRequestReasonAccident,ShippingRequestReasonAccidentListDto>();
            CreateMap<CreateOrEditShippingRequestReasonAccidentDto, ShippingRequestReasonAccident>().ReverseMap();
        }
    }
}
