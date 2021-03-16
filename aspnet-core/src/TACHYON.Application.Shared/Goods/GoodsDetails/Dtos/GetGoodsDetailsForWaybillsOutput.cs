using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GetGoodsDetailsForWaybillsOutput
    {
        public string Description { get; set; }

        public int TotalAmount { get; set; }

        public string UnitOfMeasureDisplayName { get; set; }
    }
}
