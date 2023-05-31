﻿using Abp.Application.Services.Dto;
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
        
        public int? GoodCategoryId { get; set; }
        public string Name { get; set; }
        public decimal ShipperRating { get; set; }
        public int ShipperRatingNumber { get; set; }
        public string Carrier { get; set; }
        public int? CarrierTenantId { get; set; }
        public string ShipperName { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool IsTachyonDeal { get; set; }
        public bool IsBid { get; set; }
        public bool CreatedByTachyonDealer { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCity { get; set; }
        public long TrucksTypeId { get; set; }
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
        public ShippingTypeEnum ShippingTypeId { get; set; }
        public string ShippingTypeTitle { get { return ShippingTypeId.GetEnumDescription(); } }
        public string RouteType { get { return RouteTypeId.GetEnumDescription(); } }
        public decimal? Price { get; set; }
        public string PriceOrOffer { get; set; }
        public string ShipperPriceOrOffer { get; set; }

        public string ReferenceNumber { get; set; }

        public ShippingRequestType requestType { get; set; }
        public string requestTypeTitle { get { return requestType.GetEnumDescription(); } }

        public bool IsDrafted { get; set; }
        public int NotesCount { get; set; }

        public List<ShippingRequestDestinationCitiesDto> destinationCities { get; set; }
        public string Country { get; set; }

        public string ShipperActor { get; set; }


        public string CarrierActor { get; set; }

        public ShippingRequestFlag ShippingRequestFlag { get; set; }
        public string ShippingRequestFlagTitle { get; set; }

        public string TransportType { get; set; }

        public decimal? ShipperPrice { get; set; }
        public string ShipperInvoiceNo { get; set; }

        #region Dedicated
        public TimeUnit? RentalDurationUnit { get; set; }
        public string RentalDurationUnitTitle { get; set; }
        public int RentalDuration { get; set; }
        public double ExpectedMileage { get; set; }
        public string ServiceAreaNotes { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public int NumberOfTrucks { get; set; }

        #endregion

        /// <summary>
        /// shipper Id 
        /// </summary>
        public int TenantId { get; set; }
        public bool IsDriversAndTrucksAssigned { get; set; }
        public bool CanAssignDedicatedDriversAndTrucks { get; set; }

        public long? OriginFacilityId { get; set; }

        public bool IsSaas
        {
            get => TenantId == CarrierTenantId;

        }  
        

    }
}