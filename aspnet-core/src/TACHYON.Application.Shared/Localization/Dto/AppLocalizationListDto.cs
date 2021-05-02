using Abp.Application.Services.Dto;

namespace TACHYON.Localization.Dto
{
    public class AppLocalizationListDto : EntityDto
    {
        public string MasterKey { get; set; }
        public string MasterValue { get; set; }
        public string Value { get; set; }

    }
}
