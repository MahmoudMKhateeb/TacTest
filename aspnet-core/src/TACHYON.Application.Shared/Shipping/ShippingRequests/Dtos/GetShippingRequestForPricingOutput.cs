using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForPricingOutput:EntityDto<long>
    {
        public string Shipper { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsTachyonDeal { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public string TrukType { get; set; }
        public bool IsPricing { get; set; }
        public string RangeDate { get; set; }
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public int TotalBids { get; set; }
    }
}
