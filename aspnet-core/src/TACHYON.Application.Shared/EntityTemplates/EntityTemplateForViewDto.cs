using Abp.Application.Services.Dto;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplateForViewDto : EntityDto<long>
    {
        public string SavedEntity { get; set; }
        
        public string SavedEntityId { get; set; }
        
        public int TenantId { get; set; }

        public string Type { get; set; }
    }
}