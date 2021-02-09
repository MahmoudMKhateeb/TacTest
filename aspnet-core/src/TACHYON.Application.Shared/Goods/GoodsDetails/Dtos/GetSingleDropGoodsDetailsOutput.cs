using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GetSingleDropGoodsDetailsOutput
    {
        public string Name { get; set; } 
        public string Description { get; set; }
        public int TotalAmount { get; set; }
        public string GoodCategoryDisplayName { get; set; }
        public string UnitOfMeasureDisplayName { get; set; }

        public double TotalWeight { get; set; }
        public string PackingType { get; set; }
        public int NumberOfPacking { get; set; }
    }
}
