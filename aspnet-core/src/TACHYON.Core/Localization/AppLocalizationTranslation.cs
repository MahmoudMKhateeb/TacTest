using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Localization
{
    [Table("AppLocalizationTranslations")]
    public class AppLocalizationTranslation : Entity, IEntityTranslation<AppLocalization>
    {
        [Required] public string Value { get; set; }
        public AppLocalization Core { get; set; }
        public int CoreId { get; set; }
        [Required] public string Language { get; set; }
    }
}