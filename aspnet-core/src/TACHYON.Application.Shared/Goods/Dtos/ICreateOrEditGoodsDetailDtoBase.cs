using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Goods.Dtos
{
    public interface ICreateOrEditGoodsDetailDtoBase
    {
        int Amount { get; set; }
        double Weight { get; set; }
    }
}
