using AutoMapper;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestResoneAccidentProfile: Profile
    {
        public ShippingRequestResoneAccidentProfile ()
        {
            CreateMap<ShippingRequestReasonAccidentTranslationDto, ShippingRequestReasonAccidentTranslation>().ReverseMap();
            CreateMap<ShippingRequestReasonAccident, CreateOrEditShippingRequestReasonAccidentDto>()
                .ForMember(dst => dst.Translations, opt => opt.MapFrom(src => src.Translations)).ReverseMap();



        }
    }
}
