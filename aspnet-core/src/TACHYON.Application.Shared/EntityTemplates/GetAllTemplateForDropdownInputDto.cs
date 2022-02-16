using System.ComponentModel.DataAnnotations;

namespace TACHYON.EntityTemplates
{
    public class GetAllTemplateForDropdownInputDto
    {
        [Required]
        public SavedEntityType Type { get; set; }
        
        public string ParentEntityId { get; set; }
    }
}