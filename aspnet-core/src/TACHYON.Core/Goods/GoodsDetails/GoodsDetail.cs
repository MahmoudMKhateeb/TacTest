using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Goods.GoodCategories;
using TACHYON.Shipping.ShippingRequests;
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

        //[StringLength(GoodsDetailConsts.MaxQuantityLength, MinimumLength = GoodsDetailConsts.MinQuantityLength)]
        //public virtual string Quantity { get; set; }
        /// <summary>
        /// Total Amount for this Category of Goods 
        /// </summary>
        [Required]
        public int TotalAmount { get; set; }

        /// <summary>
        /// Weight of this category of goods
        /// </summary>
        [Required]
        [StringLength(GoodsDetailConsts.MaxWeightLength, MinimumLength = GoodsDetailConsts.MinWeightLength)]
        public virtual double Weight { get; set; }

        [StringLength(GoodsDetailConsts.MaxDimentionsLength, MinimumLength = GoodsDetailConsts.MinDimentionsLength)]
        public virtual string Dimentions { get; set; } //todo  x y z 

        [Required]
        public virtual bool IsDangerousGood { get; set; }

        [StringLength(GoodsDetailConsts.MaxDangerousGoodsCodeLength, MinimumLength = GoodsDetailConsts.MinDangerousGoodsCodeLength)]
        public virtual string DangerousGoodsCode { get; set; }

        /// <summary>
        /// this category represents subcategory, which nested in base category that specefied in shipping request, subcategory is the one that has father category in GoodsCategory entity
        /// </summary>
        [Required]
        public virtual int GoodCategoryId { get; set; }

        [ForeignKey("GoodCategoryId")]
        public GoodCategory GoodCategoryFk { get; set; }

        /// <summary>
        /// unit of measure for the total amount, ex: litre
        /// </summary>
        [Required]
        public int UnitOfMeasureId { get; set; }

        [ForeignKey("UnitOfMeasureId")]
        public UnitOfMeasure UnitOfMeasureFk { get; set; }
        // public string PackingType { get; set; }
        //public int NumberOfPackingType { get; set; }

        [Required]
        public long ShippingRequestId { get; set; }

        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }

        public string PackingType { get; set; }
        public int NumberOfPacking { get; set; }


    }
}