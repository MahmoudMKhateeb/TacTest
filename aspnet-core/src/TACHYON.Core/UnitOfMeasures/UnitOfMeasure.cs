using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;

namespace TACHYON.UnitOfMeasures
{
    [Table("UnitOfMeasures")]
    public class UnitOfMeasure : FullAuditedEntity, IHasKey
    {
        // todo Setup This Entity To Be MultiLingual Entity

        [StringLength(UnitOfMeasureConsts.MaxDisplayNameLength, MinimumLength = UnitOfMeasureConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(UnitOfMeasureConsts.MaxDisplayNameLength, MinimumLength = UnitOfMeasureConsts.MinDisplayNameLength)]
        public virtual string Key { get; set; }
    }
}