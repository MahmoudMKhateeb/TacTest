using Abp.Configuration;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.MultiTenancy;
using TACHYON.Packing.PackingTypes;
using TACHYON.Routs;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutSteps;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBidStatuses;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Trailers;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.UnitOfMeasures;
using TACHYON.ShippingRequestVases;
using TACHYON.TachyonPriceOffers;
using TACHYON.Cities;

namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequests")]
    public class ShippingRequest : FullAuditedEntity<long>, IMustHaveTenant, IHasIsDrafted
    {
        public string ReferenceNumber { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        /// <summary>
        /// when it is bidding shipping request
        /// </summary>
        public virtual bool IsBid { get; set; }

        /// <summary>
        /// when it is not bidding and it is Tachyon Deal
        /// </summary>
        public virtual bool IsTachyonDeal { get; set; }
        /// <summary>
        /// Is direct request Field
        /// </summary>
        public virtual bool IsDirectRequest { get; set; }

        public ShippingRequestRouteType? RouteTypeId { get; set; }


        //city
        public virtual int? OriginCityId { get; set; }

        [ForeignKey("OriginCityId")]
        public City OriginCityFk { get; set; }

        public virtual int? DestinationCityId { get; set; }

        [ForeignKey("DestinationCityId")]
        public City DestinationCityFk { get; set; }

        /// <summary>
        /// The type of shipping request
        /// </summary>
        public ShippingRequestType RequestType { get; set; }
        /// <summary>
        /// assigned price after commission
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// when shipper accept tachyon-user price
        /// </summary>
        public bool? IsPriceAccepted { get; set; }

        /// <summary>
        /// when shipper Advance paid
        /// </summary>
        public bool? IsPrePayed { get; set; }

        /// <summary>
        /// check if this request is related with shipper invoice
        /// </summary>
        public bool IsShipperHaveInvoice { get; set; }
        /// <summary>
        /// check if this request is related with carrirer invoice
        /// </summary>
        public bool IsCarrierHaveInvoice { get; set; }
        /// <summary>
        /// when shipper reject tachyon-user price
        /// </summary>
       // public bool? IsRejected { get; set; }

        /// <summary>
        /// ShippingRequest that this ShippingRequest cloned from
        /// </summary>
        public long? FatherShippingRequestId { get; set; }

        [ForeignKey("FatherShippingRequestId")]
        public ShippingRequest FatherShippingRequestFk { get; set; }


        /// <summary>
        /// the carrier who will take this shipping request
        /// </summary>
        public int? CarrierTenantId { get; set; }

        [ForeignKey("CarrierTenantId")]
        public Tenant CarrierTenantFk { get; set; }


        /// <summary>
        /// if rout type == milk run , number of points drops, for each trip
        /// </summary>
        public int NumberOfDrops { get; set; }

        /// <summary>
        /// goods category that will be is this shipping request, which is base category that doesn't have father category
        /// </summary>
        //[Required]
        public int? GoodCategoryId { get; set; }

        [ForeignKey("GoodCategoryId")]
        public GoodCategory GoodCategoryFk { get; set; }
        /// <summary>
        /// Status of shipping request
        /// </summary>
        public ShippingRequestStatus Status { get; set; }
        /// <summary>
        /// assigned Driver
        /// </summary>
        public long? AssignedDriverUserId { get; set; }
        [ForeignKey("AssignedDriverUserId")]
        public User AssignedDriverUserFk { get; set; }
        /// <summary>
        /// if the driver make accident when he work on trip
        /// </summary>

        public bool HasAccident { get; set; }

        /// <summary>
        /// assigned Truck
        /// </summary>
        public long? AssignedTruckId { get; set; }
        [ForeignKey("AssignedTruckId")]
        public Truck AssignedTruckFk { get; set; }


        public int NumberOfTrips { get; set; }
        public int TotalsTripsAddByShippier { get; set; }
        public DateTime? StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public double TotalWeight { get; set; }
        // todo make sure those are nullable

        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }

        [ForeignKey("TransportTypeId")]
        public TransportType TransportTypeFk { get; set; }
        public virtual long? TrucksTypeId { get; set; }
        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }

        public virtual int? CapacityId { get; set; }
        [ForeignKey("CapacityId")]
        public Capacity CapacityFk { get; set; }

        #endregion
        public int? PackingTypeId { get; set; }

        [ForeignKey("PackingTypeId")]
        public PackingType PackingTypeFk { get; set; }

        public int NumberOfPacking { get; set; }

        public int ShippingTypeId { get; set; }

        [ForeignKey("ShippingTypeId")]
        public ShippingType ShippingTypeFk { get; set; }

        /// <summary>
        /// This field describes if the shipping request is draft or not, draft means incomplete request
        /// </summary>
        public bool IsDrafted { get; set; }

        public int DraftStep { get; set; }
        [StringLength(500)]
        public string CancelReason { get; set; }
        #region Bids Data
        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }
        public int TotalBids { get; set; }

        public DateTime? CloseBidDate { get; set; }

        public int TotalOffers { get; set; }
        public ICollection<ShippingRequestBid> ShippingRequestBids { get; set; }

        //Vases for all trips 
        public ICollection<ShippingRequestVas> ShippingRequestVases { get; set; }

        //trips collection
        public ICollection<ShippingRequestTrip> ShippingRequestTrips { get; set; }
        #endregion

        #region Commission?
        public decimal VatSetting { get; set; }
        /// <summary>
        /// total of price after commission, before add Vat settings
        /// </summary>
        public decimal SubTotalAmount { get; set; }
        /// <summary>
        /// Amount of vat, PriceSubTotal*Vat/100
        /// </summary>
        public decimal VatAmount { get; set; }
        public decimal PercentCommissionSetting { get; set; }
        public decimal CommissionValueSetting { get; set; }
        public decimal MinValueCommissionSetting { get; set; }
        /// <summary>
        /// total commission always sum of ActualPercentCommission + ActualCommissionValue + ActualMinCommission
        /// </summary>
        public decimal TotalCommission { get; set; }
        // profit of final price calculated after all commissions + base carrier price 
       // public decimal TachyonDealerProfit { get; set; }
        //this field special to tachyonDealer,describes accepted carrier price coming from bidding or direct request, or other; the price source defines in TachyonPriceOffer
        public decimal CarrierPrice { get; set; }
        /// <summary>
        /// type of carrier price, describes from where the price comes from, bidding, direct request, ..
        /// </summary>
        public CarrierPriceType CarrierPriceType { get; set; }
        public decimal ActualPercentCommission { get; set; }
        public decimal ActualCommissionValue { get; set; }
        public decimal ActualMinCommissionValue { get; set; }
        #endregion
        public void Close()
        {
            BidStatus = ShippingRequestBidStatus.Closed;
            CloseBidDate = Clock.Now;
            Status = ShippingRequestStatus.Expired;
        }

        public void Start()
        {
            BidStatus = ShippingRequestBidStatus.OnGoing;
            BidStartDate = Clock.Now;
        }

    }
}