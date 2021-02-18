
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GoodsDetailDto : EntityDto<long>
    {
        //public string Name { get; set; }

        public string Description { get; set; }

        public int Amount { get; set; }

        public double Weight { get; set; }

        public string Dimentions { get; set; }

        public bool IsDangerousGood { get; set; }

        public string DangerousGoodsCode { get; set; }

        public int? GoodCategoryId { get; set; }
        public long RoutPointId { get; set; }

    }
}