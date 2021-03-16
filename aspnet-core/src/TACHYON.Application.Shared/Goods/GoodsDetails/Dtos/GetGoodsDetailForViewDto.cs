using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Goods.GoodsDetails.Dtos
{
    public class GetGoodsDetailForViewDto
    {
        public GoodsDetailDto GoodsDetail { get; set; }
        public string RoutPointDisplayName { get; set; }
        public string GoodCategoryDisplayName { get; set; }

        public string UnitOfMeasureDisplayName { get; set; }
    }
}