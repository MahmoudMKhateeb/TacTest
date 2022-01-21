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
            CreateMap<TerminologieEdition, TerminologieEditionDto>()
                .ForMember(dst => dst.Edition, opt => opt.MapFrom(src => src.Edition.DisplayName));
            CreateMap<TerminologiePage, TerminologiePageDto>();
            CreateMap<AppLocalization, AppLocalizationForViewDto>()
                .ForMember(dst => dst.Translations, opt => opt.MapFrom(src => src.Translations))
                .ForMember(dst => dst.TerminologieEditions, opt => opt.MapFrom(src => src.TerminologieEditions))
                .ForMember(dst => dst.TerminologiePages, opt => opt.MapFrom(src => src.TerminologiePages));
        }
    }
}