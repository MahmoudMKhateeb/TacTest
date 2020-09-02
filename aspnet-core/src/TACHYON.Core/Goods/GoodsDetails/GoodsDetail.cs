using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Goods.GoodCategories;
using TACHYON.UnitOfMeasures;

namespace TACHYON.Goods.GoodsDetails
{
    [Table("GoodsDetails")]
    public class GoodsDetail : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [Required]
        [StringLength(GoodsDetailConsts.MaxNameLength, MinimumLength = GoodsDetailConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [StringLength(GoodsDetailConsts.MaxDescriptionLength, MinimumLength = GoodsDetailConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        [StringLength(GoodsDetailConsts.MaxQuantityLength, MinimumLength = GoodsDetailConsts.MinQuantityLength)]
        public virtual string Quantity { get; set; }

        [StringLength(GoodsDetailConsts.MaxWeightLength, MinimumLength = GoodsDetailConsts.MinWeightLength)]
        public virtual string Weight { get; set; }

        [StringLength(GoodsDetailConsts.MaxDimentionsLength, MinimumLength = GoodsDetailConsts.MinDimentionsLength)]
        public virtual string Dimentions { get; set; }

        public virtual bool IsDangerousGood { get; set; }

        [StringLength(GoodsDetailConsts.MaxDangerousGoodsCodeLength, MinimumLength = GoodsDetailConsts.MinDangerousGoodsCodeLength)]
        public virtual string DangerousGoodsCode { get; set; }


        public virtual int? GoodCategoryId { get; set; }

        [ForeignKey("GoodCategoryId")]
        public GoodCategory GoodCategoryFk { get; set; }

        public int UnitOfMeasureId { get; set; }
        [ForeignKey("UnitOfMeasureId")]
        public UnitOfMeasure UnitOfMeasureFk { get; set; }

    }
}