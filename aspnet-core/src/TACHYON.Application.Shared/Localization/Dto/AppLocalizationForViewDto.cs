using System.Collections.Generic;

namespace TACHYON.Localization.Dto
{
    public  class AppLocalizationForViewDto
    {
        public string MasterKey { get; set; }
        public string MasterValue { get; set; }
        public string Value { get; set; }
        public List<TerminologieEditionDto> TerminologieEditions { get; set; }
        public List<TerminologiePageDto> TerminologiePages { get; set; }

        public List<AppLocalizationTranslationDto> Translations { get; set; }


    }
}
