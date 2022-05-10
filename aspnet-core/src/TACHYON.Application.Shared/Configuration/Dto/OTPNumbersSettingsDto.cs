using System.ComponentModel.DataAnnotations;

namespace TACHYON.Configuration.Dto
{
    public class OtpNumbersSettingsDto
    {
        
        [RegularExpression(@"^\d+(?:;\d+)*$")]
        public string IgnoredOtpNumbers { get; set; }
    }
}