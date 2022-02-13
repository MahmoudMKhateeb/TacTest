using System.ComponentModel.DataAnnotations;
using TACHYON.Dto;

namespace TACHYON.EntityTemplates
{
    public class GetEntityTemplateInputDto : PagedSortedAndFilteredInputDto
    {
        public SavedEntityType? Type { get; set; }
    }
}