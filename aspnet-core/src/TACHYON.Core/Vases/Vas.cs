using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;

namespace TACHYON.Vases
{
    [Table("Vases")]
    public class Vas : FullAuditedEntity, IMultiLingualEntity<VasTranslation>, IHasName
    {
        [StringLength(VasConsts.MaxNameLength, MinimumLength = VasConsts.MinNameLength)]
        public virtual string Name { get; set; }

        public virtual bool HasAmount { get; set; }

        public virtual bool HasCount { get; set; }

        public ICollection<VasTranslation> Translations { get; set; }
    }
}