using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.EntityTemplates
{
    public class EntityTemplate : FullAuditedEntity<long>, IMustHaveTenant
    {
        [Required]
        public string SavedEntity { get; set; }
        
        public string SavedEntityId { get; set; }
        
        public int TenantId { get; set; }

        public SavedEntityType EntityType { get; set; }
    }
}