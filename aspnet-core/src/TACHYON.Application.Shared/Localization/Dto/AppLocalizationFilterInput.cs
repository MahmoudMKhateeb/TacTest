using Abp.Application.Services.Dto;
using TACHYON.Terminologies;

namespace TACHYON.Localization.Dto
{
    public  class AppLocalizationFilterInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int? EditionId { get; set; }
        public string Page { get; set; }
        public TerminologyPlatForm? PlatForm { get; set; }
        public TerminologyAppVersion? AppVersion { get; set; }
        public TerminologyVersion? Version { get; set; }
        public TerminologySection? Section { get; set; }


    }
}
