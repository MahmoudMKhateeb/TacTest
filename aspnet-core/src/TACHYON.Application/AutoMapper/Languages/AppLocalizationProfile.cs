using AutoMapper;
using TACHYON.Localization;
using TACHYON.Localization.Dto;

namespace TACHYON.AutoMapper.Languages
{
    public class AppLocalizationProfile : Profile
    {
        public AppLocalizationProfile()
        {
            CreateMap<AppLocalizationTranslation, AppLocalizationTranslationDto>().ReverseMap();
            CreateMap<AppLocalization, CreateOrEditAppLocalizationDto>()
                 .ForMember(dst => dst.Translations, opt => opt.MapFrom(src => src.Translations)).ReverseMap();
        }
    }
}
