using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        /// the tenant id of the tenant that create template instead of entity owner
        /// in our case the CreatorTenantId is the Tachyon dealer tenant id 
        public int? CreatorTenantId { get; set; }
        public SavedEntityType EntityType { get; set; }
    }
}