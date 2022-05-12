using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.PickingTypes
{
    [Table("PickingTypes")]
    public class PickingType : FullAuditedEntity
    {
        [Required]
        [StringLength(PickingTypeConsts.MaxDisplayNameLength, MinimumLength = PickingTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }
    }
}