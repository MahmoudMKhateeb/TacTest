using System.ComponentModel.DataAnnotations;
namespace TACHYON.Localization.Dto
{
    public class AppLocalizationTranslationDto
    {
        [Required]
        public string Value { get; set; }
        [Required]
        public string Language { get; set; }
        public string Icon { get; set; }
        public string DisplayName { get; set; }
    }
}
