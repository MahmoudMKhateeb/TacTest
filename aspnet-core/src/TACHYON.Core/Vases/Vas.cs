using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;

namespace TACHYON.Vases
{
    [Table("Vases")]
    public class Vas : FullAuditedEntity, IMultiLingualEntity<VasTranslation>
    {
        [StringLength(VasConsts.MaxNameLength, MinimumLength = VasConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [StringLength(VasConsts.MaxDisplayNameLength, MinimumLength = VasConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public virtual bool HasAmount { get; set; }

        public virtual bool HasCount { get; set; }

        public ICollection<VasTranslation> Translations { get; set; }
    }
}