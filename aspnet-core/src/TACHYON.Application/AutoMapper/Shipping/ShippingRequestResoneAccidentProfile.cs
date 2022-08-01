using AutoMapper;
using System.Globalization;
using System.Linq;
using TACHYON.Extension;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping
{
    public class ShippingRequestResoneAccidentProfile : Profile
    {
        public ShippingRequestResoneAccidentProfile()
        {
            CreateMap<ShippingRequestReasonAccidentTranslationDto, ShippingRequestReasonAccidentTranslation>()
                .ReverseMap();
            CreateMap<ShippingRequestReasonAccident, CreateOrEditShippingRequestReasonAccidentDto>()
                .ForMember(dst => dst.Translations, opt => opt.MapFrom(src => src.Translations)).ReverseMap();
            CreateMap<ShippingRequestReasonAccident, ShippingRequestAccidentReasonLookupDto>()
                .ForMember(dst => dst.IsOther, opt => opt.MapFrom(src => src.ContainsOther()))
                .ForMember(x => x.DisplayName, x => x.MapFrom(src => src.Key));

        }
    }
}