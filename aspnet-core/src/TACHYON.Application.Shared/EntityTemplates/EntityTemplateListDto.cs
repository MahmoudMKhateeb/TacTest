using Abp.Application.Services.Dto;
using System;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplateListDto : EntityDto<long>
    {
        public string SavedEntityId { get; set; }
        
        public string TemplateName { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public string EntityTypeTitle { get; set; }
        
        public SavedEntityType EntityType { get; set; }
    }
}