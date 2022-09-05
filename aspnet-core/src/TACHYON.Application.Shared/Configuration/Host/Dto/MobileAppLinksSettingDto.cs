using System.ComponentModel.DataAnnotations;

namespace TACHYON.Configuration.Host.Dto
{
    public class MobileAppLinksSettingDto
    {
        [Required]
        public string IosAppLink { get; set; }

        [Required]
        public string AndroidAppLink { get; set; }
    }
}