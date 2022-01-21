using Abp.Application.Services.Dto;
using TACHYON.Terminologies;

namespace TACHYON.Localization.Dto
{
    public class AppLocalizationListDto : EntityDto
    {
        public string MasterKey { get; set; }
        public string MasterValue { get; set; }
        public string Value { get; set; }
        public TerminologyPlatForm PlatForm { get; set; }
        public TerminologyAppVersion AppVersion { get; set; }
        public TerminologyVersion Version { get; set; }
        public TerminologySection Section { get; set; }


        public string PlatFormTitle { get { return PlatForm.GetEnumDescription(); } }
        public string AppVersionTitle { get { return AppVersion.GetEnumDescription(); } }
        public string VersionTitle { get { return Version.GetEnumDescription(); } }
        public string SectionTitle { get { return Section.GetEnumDescription(); } }
    }
}