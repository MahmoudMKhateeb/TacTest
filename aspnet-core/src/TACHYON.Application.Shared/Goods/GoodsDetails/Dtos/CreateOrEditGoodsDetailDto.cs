
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class CreateOrEditGoodsDetailDto : EntityDto<long?>
    {

        //[Required]
        //[StringLength(GoodsDetailConsts.MaxNameLength, MinimumLength = GoodsDetailConsts.MinNameLength)]
        //public string Name { get; set; }


        [StringLength(GoodsDetailConsts.MaxDescriptionLength, MinimumLength = GoodsDetailConsts.MinDescriptionLength)]
        public string Description { get; set; }


        public int TotalAmount { get; set; }

        [Required]
        public double Weight { get; set; }


        [StringLength(GoodsDetailConsts.MaxDimentionsLength, MinimumLength = GoodsDetailConsts.MinDimentionsLength)]
        public string Dimentions { get; set; }

        [Required]
        public bool IsDangerousGood { get; set; }


        [StringLength(GoodsDetailConsts.MaxDangerousGoodsCodeLength, MinimumLength = GoodsDetailConsts.MinDangerousGoodsCodeLength)]
        public string DangerousGoodsCode { get; set; }

        [Required]
        public int? GoodCategoryId { get; set; }

        [Required]
        public int UnitOfMeasureId { get; set; }

        [Required]
        public long RoutPointId { get; set; }
    }
}