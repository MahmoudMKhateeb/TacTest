﻿using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers.Dto
{
    public class GetShippingRequestForPriceOfferListDto : EntityDto<long>
    {
        public long? DirectRequestId { get; set; }
        public long? OfferId { get; set; }
        public string Name { get; set; }
        public string Carrier { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool IsTachyonDeal { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public string TrukType { get; set; }
        public bool isPriced { get; set; }
        public string RemainingDays { get; set; }
        public string RangeDate { get; set; }
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public int TotalOffers { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }
        public ShippingRequestStatus Status { get; set; }
        public ShippingRequestDirectRequestStatus DirectRequestStatus;
        public string StatusTitle { get; set; }
        public string BidStatusTitle { get; set; }
        public string DirectRequestStatusTitle { get; set; }
        public ShippingRequestRouteType RouteTypeId { get; set; }
        public string RouteType { get { return RouteTypeId.GetEnumDescription(); }}
        public decimal? Price { get; set; }

    }
 
}