using System.Collections.Generic;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class GetPackingTypeForViewDto
    {
        public virtual string DisplayName { get; set; }

        public virtual string Description { get; set; }
        public List<PackingTypeTranslationDto> PackingTypeTranslations { get; set; }

    }
}