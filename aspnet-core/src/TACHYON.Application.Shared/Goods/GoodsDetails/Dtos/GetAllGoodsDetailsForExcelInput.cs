using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GetAllGoodsDetailsForExcelInput
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public string QuantityFilter { get; set; }

        public string WeightFilter { get; set; }

        public string DimentionsFilter { get; set; }

        public int IsDangerousGoodFilter { get; set; }

        public string DangerousGoodsCodeFilter { get; set; }


        public string GoodCategoryDisplayNameFilter { get; set; }


    }
}