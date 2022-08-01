using Abp.Application.Services.Dto;
using System;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplateForViewDto : EntityDto<long>
    {
        public string SavedEntity { get; set; }
        
        public string SavedEntityId { get; set; }
        
        public int TenantId { get; set; }
        
        public string TemplateName { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public string EntityTypeTitle { get; set; }
        
        public SavedEntityType EntityType { get; set; }
    }
}