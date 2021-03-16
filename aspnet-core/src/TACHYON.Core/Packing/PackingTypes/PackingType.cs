using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Packing.PackingTypes
{
    [Table("PackingTypes")]
    public class PackingType : FullAuditedEntity
    {

        [Required]
        [StringLength(PackingTypeConsts.MaxDisplayNameLength, MinimumLength = PackingTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(PackingTypeConsts.MaxDescriptionLength, MinimumLength = PackingTypeConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

    }
}