using Abp.Application.Services.Dto;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplateListDto : EntityDto<long>
    {
        public string SavedEntity { get; set; }
        
        public string SavedEntityId { get; set; }
        
        public string EntityType { get; set; }
    }
}