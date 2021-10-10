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
        // This Column Contain Original DisplayName Value
        [Required]
        [StringLength(GoodCategoryConsts.MaxDisplayNameLength, MinimumLength = GoodCategoryConsts.MinDisplayNameLength)]
        public string Key { get; set; }

        public int? FatherId { get; set; }

        [ForeignKey("FatherId")]
        public GoodCategory FatherFk { get; set; }
        public ICollection<GoodCategoryTranslation> Translations { get; set; }
        public bool IsActive { get; set; } = true;
        public string BayanIntegrationId { get; set; }

        public ICollection<GoodCategory> GoodCategories { get; set; }

    }
}