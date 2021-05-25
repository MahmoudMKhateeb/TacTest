using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Goods.GoodCategories
{
    [Table("GoodCategories")]
    [Audited]
    public class GoodCategory : FullAuditedEntity, IMultiLingualEntity<GoodCategoryTranslation>
    {

        //[Required]
        //[StringLength(GoodCategoryConsts.MaxDisplayNameLength, MinimumLength = GoodCategoryConsts.MinDisplayNameLength)]
        //public virtual string DisplayName { get; set; }

        public int? FatherId { get; set; }

        [ForeignKey("FatherId")]
        public GoodCategory FatherFk { get; set; }
        public ICollection<GoodCategoryTranslation> Translations { get; set; }
    }
}