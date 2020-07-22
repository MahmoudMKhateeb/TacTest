using Abp.Localization;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Localization.Dto
{
    public class SetDefaultLanguageInput
    {
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string Name { get; set; }
    }
}