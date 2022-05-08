using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class PlateTypeTranslationDto
    {
        //display name for specific language
        public string DisplayName { get; set; }

        // language code
        public string Language { get; set; }

        //this field is angular helper for language name
        public string LanguageDisplayName { get; set; }

        //icon for language, for angular helper
        public string icon { get; set; }
    }
}