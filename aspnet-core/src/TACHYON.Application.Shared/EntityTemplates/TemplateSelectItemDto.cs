using System.Collections.Generic;

namespace TACHYON.EntityTemplates
{
    public class TemplateSelectItemDto
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public SavedEntityType Type { get; set; }
    }

    public class TemplateSelectItemGroupDto
    {
        public string TypeTitle { get; set; }

        public List<TemplateSelectItemDto> Templates { get; set; }
    }
}