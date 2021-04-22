using Abp.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Localization
{
    [Table("AppLocalizations")]
    public  class AppLocalization:Entity, IMultiLingualEntity<AppLocalizationTranslation>
    {
        [Required]
        [DataType(dataType:DataType.Text)]
        public string MasterKey { get; set; }
        [Required]
        [DataType(dataType: DataType.MultilineText)]
        public string MasterValue { get; set; }
        public ICollection<AppLocalizationTranslation> Translations { get; set; }

    }
}
