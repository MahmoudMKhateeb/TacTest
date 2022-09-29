using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.PriceOffers.Dto
{
    /// <summary>
    /// mapped from ShippingRequest
    /// </summary>
    public class GetShippingRequestForPriceOfferListDto : EntityDto<long>
    {
        public long? DirectRequestId { get; set; }
        public long? OfferId { get; set; }
        public long? BidNormalPricePackageId { get; set; }
        public int? GoodCategoryId { get; set; }
        public string Name { get; set; }
        public decimal ShipperRating { get; set; }
        public int ShipperRatingNumber { get; set; }
        public string Carrier { get; set; }
        public int? CarrierTenantId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool IsTachyonDeal { get; set; }
        public bool IsBid { get; set; }
        public bool CreatedByTachyonDealer { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public string TruckType { get; set; }
        public bool isPriced { get; set; }
        public string RemainingDays { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }

        //x public string RangeDate { get; set; } 
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public int NumberOfCompletedTrips { get; set; }
        public int TotalsTripsAddByShippier { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public int TotalOffers { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }
        public ShippingRequestStatus Status { get; set; }
        public ShippingRequestDirectRequestStatus DirectRequestStatus { get; set; }
        public PriceOfferStatus OfferStatus { get; set; }
        public string StatusTitle { get; set; }
        public string BidStatusTitle { get; set; }
        public string DirectRequestStatusTitle { get; set; }
        public ShippingRequestRouteType RouteTypeId { get; set; }
        public string RouteType { get { return RouteTypeId.GetEnumDescription(); } }
        public decimal? Price { get; set; }

        public string ReferenceNumber { get; set; }

        public ShippingRequestType requestType { get; set; }
        public string requestTypeTitle { get { return requestType.GetEnumDescription(); } }

        public bool IsDrafted { get; set; }
        public int NotesCount { get; set; }

        public List<ShippingRequestDestinationCitiesDto> destinationCities { get; set; }

        /// <summary>
        /// shipper Id 
        /// </summary>
        public int TenantId { get; set; }

        public bool IsSaas
        {
            get => TenantId == CarrierTenantId;

        }
    }
}