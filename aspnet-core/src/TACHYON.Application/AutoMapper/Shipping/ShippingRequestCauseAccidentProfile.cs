using AutoMapper;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestCauseAccidentProfile: Profile
    {
        public ShippingRequestCauseAccidentProfile ()
        {
            CreateMap<ShippingRequestCauseAccident,ShippingRequestCauseAccidentListDto>();
            CreateMap<CreateOrEditShippingRequestCauseAccidentDto, ShippingRequestCauseAccident>().ReverseMap();
        }
    }
}
