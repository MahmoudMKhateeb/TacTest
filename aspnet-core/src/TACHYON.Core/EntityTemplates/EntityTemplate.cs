using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplate : FullAuditedEntity<long>, IMustHaveTenant
    {
        [Required]
        [StringLength(2,MinimumLength = 150)]
        public string TemplateName { get; set; }
        
        [Required]
        public string SavedEntity { get; set; }
        
        public string SavedEntityId { get; set; }
        
        public int TenantId { get; set; }

        public SavedEntityType EntityType { get; set; }
    }
}