using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Timing;
using TACHYON.Authorization.Users;
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

namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequests")]
    public class ShippingRequest : FullAuditedEntity<long>, IMustHaveTenant
    {
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

        public int RouteId { get; set; }

        [ForeignKey("RouteId")]
        public Route RouteFk { get; set; }
        /// <summary>
        /// Tachyon user price
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
        /// Related with invoice id
        /// </summary>
        public long? InvoiceId { get; set; }
        /// <summary>
        /// when shipper reject tachyon-user price
        /// </summary>
        public bool? IsRejected { get; set; }

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

        //public bool StageOneFinish { get; set; }
        //public bool StageTowFinish { get; set; }
        //public bool StageThreeFinish { get; set; }


        /// <summary>
        /// goods category that will be is this shipping request, which is base category that doesn't have father category
        /// </summary>
        [Required]
        public int GoodCategoryId { get; set; }

        [ForeignKey("GoodCategoryId")]
        public GoodCategory GoodCategoryFk { get; set; }

        /// <summary>
        /// g-#409 
        /// </summary>
        //public int ShippingRequestStatusId { get; set; }
        //[ForeignKey("ShippingRequestStatusId")]
        //public ShippingRequestStatuses.ShippingRequestStatus ShippingRequestStatusFk { get; set; }

        public ShippingRequestStatus Status { get; set; }
        /// <summary>
        /// assigned Driver
        /// </summary>
        public long? AssignedDriverUserId { get; set; }
        [ForeignKey("AssignedDriverUserId")]
        public User AssignedDriverUserFk { get; set; }
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
        public virtual long TrucksTypeId { get; set; }
        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }

        public virtual int? CapacityId { get; set; }
        [ForeignKey("CapacityId")]
        public Capacity CapacityFk { get; set; }

        #endregion
        public int PackingTypeId { get; set; }

        [ForeignKey("PackingTypeId")]
        public PackingType PackingTypeFk { get; set; }

        public int NumberOfPacking { get; set; }

        public int ShippingTypeId { get; set; }

        [ForeignKey("ShippingTypeId")]
        public ShippingType ShippingTypeFk { get; set; }


        #region Bids Data
        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }

        //public int? ShippingRequestBidStatusId { get; set; }

        //[ForeignKey("ShippingRequestBidStatusId")]
        //public ShippingRequestBidStatus ShippingRequestBidStatusFK { get; set; }
        public DateTime? CloseBidDate { get; set; }

        public ICollection<ShippingRequestBid> ShippingRequestBids { get; set; }

        //Vases for all trips 
        public ICollection<ShippingRequestVas> ShippingRequestVases { get; set; }

        //trips collection
        public ICollection<ShippingRequestTrip> ShippingRequestTrips { get; set; }
        #endregion

        public ShippingRequest()
        {
            //StageOneFinish = false;
            //StageTowFinish = false;
            //StageThreeFinish = false;
        }
        public void Close()
        {
            BidStatus = ShippingRequestBidStatus.Closed;
            CloseBidDate = Clock.Now;
        }

        public void Start()
        {
            BidStatus = ShippingRequestBidStatus.OnGoing;
            BidStartDate = Clock.Now;
        }
    }
}