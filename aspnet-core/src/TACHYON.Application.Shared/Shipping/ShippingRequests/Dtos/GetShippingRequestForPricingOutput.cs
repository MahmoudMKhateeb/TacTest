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
        public decimal ShipperRating { get; set; }
        
        public decimal FacilitiesRatingAverage { get; set; }
        
        public int FacilitiesRatingCount { get; set; }
        public int ShipperRatingNumber { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsTachyonDeal { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public List<ShippingRequestDestinationCitiesDto> DestinationCities { get; set; }
        public string TrukType { get; set; }
        public bool IsPriced { get; set; }
        public string RangeDate { get; set; }
        public int NumberOfDrops { get; set; }
        public int NumberOfTrips { get; set; }
        public int NumberOfTrucks { get; set; }
        public string GoodsCategory { get; set; }
        public double TotalWeight { get; set; }
        public int TotalBids { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }
        public ShippingRequestStatus Status { get; set; }
        public ShippingRequestFlag shippingRequestFlag { get; set; }
        public ICollection<PriceOfferItemDto> Items = new List<PriceOfferItemDto>();

        /// <summary>
        /// shipper Id
        /// </summary>
        public int TenantId { get; set; }

        public long? MatchingPricePackageId { get; set; }

        #region Port movements

        public long? OriginFacilityId { get; set; }

        public string OriginFacilityTitle { get; set; }
        public ShippingTypeEnum ShippingTypeId { get; set; }
        public string ShippingTypeTitle { get; set; }
        public string RoundTripTitle { get; set; }
        public string PackingTypeTitle { get; set; }

        #endregion
    }
}