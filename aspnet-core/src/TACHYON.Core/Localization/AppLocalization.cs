using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Terminologies;

namespace TACHYON.Localization
{
    [Table("AppLocalizations")]
    public  class AppLocalization:Entity, IMultiLingualEntity<AppLocalizationTranslation>
    {
        [Required]
        [DataType(dataType:DataType.Text)]
        public string MasterKey { get; set; }
        [DataType(dataType: DataType.MultilineText)]
        public  string MasterValue { get; set; }
        public ICollection<AppLocalizationTranslation> Translations { get; set; }
        public List<TerminologieEdition> TerminologieEditions { get; set; } 
        public List<TerminologiePage> TerminologiePages { get; set; } 

        public TerminologyPlatForm PlatForm { get; set; }
        public TerminologyAppVersion AppVersion { get; set; }
        public TerminologyVersion Version { get; set; }
        public AppLocalization()
        {
            TerminologieEditions = new List<TerminologieEdition>();
            TerminologiePages = new List<TerminologiePage>();
        }

    }
}
