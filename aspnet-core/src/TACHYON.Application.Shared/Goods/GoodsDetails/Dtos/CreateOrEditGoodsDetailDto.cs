using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using TACHYON.Goods.Dtos;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class CreateOrEditGoodsDetailDto : EntityDto<long?>, ICreateOrEditGoodsDetailDtoBase
    {
        [Required]
        [StringLength(GoodsDetailConsts.MaxDescriptionLength, MinimumLength = GoodsDetailConsts.MinDescriptionLength)]
        public string Description { get; set; }


        public int Amount { get; set; }

        [Required] public double Weight { get; set; }


        [StringLength(GoodsDetailConsts.MaxDimentionsLength, MinimumLength = GoodsDetailConsts.MinDimentionsLength)]
        public string Dimentions { get; set; }

        [Required] public bool IsDangerousGood { get; set; }


        [StringLength(GoodsDetailConsts.MaxDangerousGoodsCodeLength,
            MinimumLength = GoodsDetailConsts.MinDangerousGoodsCodeLength)]
        public string DangerousGoodsCode { get; set; }

        public int? DangerousGoodTypeId { get; set; }

        public int? GoodCategoryId { get; set; }

        [Required] public int UnitOfMeasureId { get; set; }
        public string OtherUnitOfMeasureName { get; set; }

        public long? RoutPointId { get; set; }
    }
}