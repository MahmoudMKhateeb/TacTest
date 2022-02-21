using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Goods.Dtos;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ImportGoodsDetailsDto: ICreateOrEditGoodsDetailDtoBase
    {
        public string TripReference { get; set; }
        public int ShippingRequestTripId { get; set; }
        public string PointReference { get; set; }
        public long RoutPointId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public double Weight { get; set; }
        public string Dimentions { get; set; }
        public bool IsDangerousGood { get; set; }
        public string DangerousGoodsCode { get; set; }
        public int? DangerousGoodTypeId { get; set; }
        public string DangerousGoodsType { get; set; }
        public int? GoodCategoryId { get; set; }
        public string GoodsSubCategory { get; set; }
        public string OtherGoodsCategoryName { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Exception { get; set; }
    }
}
