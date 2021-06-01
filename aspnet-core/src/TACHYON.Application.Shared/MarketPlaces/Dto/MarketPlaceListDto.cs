using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.MarketPlaces.Dto
{
    public class MarketPlaceListDto : EntityDto<long>, IHasCreationTime
    {
        public string Shipper { get; set; }
        public DateTime CreationTime { get; set; }
        public  bool IsTachyonDeal { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public int TotalBids { get; set; }
        public ShippingRequestRouteType RouteTypeId { get; set; }
        public string RouteType { get { return RouteTypeId.GetEnumDescription(); }}
        public ShippingRequestBidStatus BidStatus { get; set; }
        public string BidStatusTitle { get { return BidStatus.GetEnumDescription(); } }
        public ShippingRequestCarrierPricingDto CarrierPricing { get; set; }

    }
}
