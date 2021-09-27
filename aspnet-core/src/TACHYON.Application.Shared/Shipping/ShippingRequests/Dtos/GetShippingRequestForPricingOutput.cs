using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using TACHYON.PriceOffers.Dto;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    /// <summary>
    /// mapped from ShippingRequest
    /// </summary>
    public class GetShippingRequestForPricingOutput : EntityDto<long>
    {
        public long OfferId { get; set; }
        public string Shipper { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsTachyonDeal { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public string TrukType { get; set; }
        public bool IsPriced { get; set; }
        public string RangeDate { get; set; }
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public int TotalBids { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }
        public ShippingRequestStatus Status { get; set; }
        public ICollection<PriceOfferItemDto> Items = new List<PriceOfferItemDto>();

        /// <summary>
        /// shipper Id
        /// </summary>
        public int TenantId { get; set; }
    }
}