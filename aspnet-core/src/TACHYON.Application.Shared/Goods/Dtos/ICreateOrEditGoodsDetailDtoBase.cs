using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Goods.Dtos
{
    public interface ICreateOrEditGoodsDetailDtoBase
    {
        int Amount { get; set; }
        double Weight { get; set; }
        long RoutPointId { get; set; }
        string Description { get; set; }
        string Dimentions { get; set; }
        bool IsDangerousGood { get; set; }
        string DangerousGoodsCode { get; set; }
        int? DangerousGoodTypeId { get; set; }
        int? GoodCategoryId { get; set; }
        int UnitOfMeasureId { get; set; }
        string Exception { get; set; }

    }
}
