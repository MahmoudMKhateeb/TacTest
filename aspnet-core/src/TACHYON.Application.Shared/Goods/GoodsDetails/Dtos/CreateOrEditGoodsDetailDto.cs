
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class CreateOrEditGoodsDetailDto : EntityDto<long?>
    {

        [Required]
        [StringLength(GoodsDetailConsts.MaxNameLength, MinimumLength = GoodsDetailConsts.MinNameLength)]
        public string Name { get; set; }


        [StringLength(GoodsDetailConsts.MaxDescriptionLength, MinimumLength = GoodsDetailConsts.MinDescriptionLength)]
        public string Description { get; set; }


        [StringLength(GoodsDetailConsts.MaxQuantityLength, MinimumLength = GoodsDetailConsts.MinQuantityLength)]
        public string Quantity { get; set; }


        [StringLength(GoodsDetailConsts.MaxWeightLength, MinimumLength = GoodsDetailConsts.MinWeightLength)]
        public string Weight { get; set; }


        [StringLength(GoodsDetailConsts.MaxDimentionsLength, MinimumLength = GoodsDetailConsts.MinDimentionsLength)]
        public string Dimentions { get; set; }


        public bool IsDangerousGood { get; set; }


        [StringLength(GoodsDetailConsts.MaxDangerousGoodsCodeLength, MinimumLength = GoodsDetailConsts.MinDangerousGoodsCodeLength)]
        public string DangerousGoodsCode { get; set; }


        public int? GoodCategoryId { get; set; }


    }
}