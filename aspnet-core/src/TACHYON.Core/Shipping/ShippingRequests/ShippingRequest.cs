using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Authorization.Users;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.MultiTenancy;
using TACHYON.Routs;
using TACHYON.Routs.RoutSteps;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.Trailers;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;
using TACHYON.Trucks.TruckCategories.TransportSubtypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckSubtypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequests")]
    public class ShippingRequest : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int RouteId { get; set; }
        public virtual decimal Vas { get; set; }


        /// <summary>
        /// when it is bidding shipping request
        /// </summary>
        public virtual bool IsBid { get; set; }

        /// <summary>
        /// when it is not bidding and it is Tachyon Deal
        /// </summary>
        public virtual bool IsTachyonDeal { get; set; }

        [ForeignKey("RouteId")]
        public Route RouteFk { get; set; }


        /// <summary>
        /// shipping request route steps that are declare the shipment steps
        /// </summary>
        public ICollection<RoutStep> RoutSteps { get; set; }

        /// <summary>
        /// Tachyon user price
        /// </summary>
        public decimal? Price { get; set; }


        /// <summary>
        /// when shipper accept tachyon-user price
        /// </summary>
        public bool? IsPriceAccepted { get; set; }

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
        /// if rout type == milk run , number of route steps drops
        /// </summary>
        public int NumberOfDrops { get; set; }

        public bool StageOneFinish { get; set; }
        public bool StageTowFinish { get; set; }
        public bool StageThreeFinish { get; set; }


        /// <summary>
        /// goods category that will be is this shipping request
        /// </summary>
        public int GoodCategoryId { get; set; }
        [ForeignKey("GoodCategoryId")]
        public GoodCategory GoodCategoryFk { get; set; }

        /// <summary>
        /// g-#409 
        /// </summary>
        public int ShippingRequestStatusId { get; set; }
        [ForeignKey("ShippingRequestStatusId")]
        public ShippingRequestStatus ShippingRequestStatusFk { get; set; }


        /// <summary>
        /// assigned Driver
        /// </summary>
        public long? AssignedDriverUserId { get; set; }
        [ForeignKey("AssignedDriverUserId")]
        public User AssignedDriverUserFk { get; set; }
        /// <summary>
        /// assigned Truck
        /// </summary>
        public Guid? AssignedTruckId { get; set; }
        [ForeignKey("AssignedTruckId")]
        public Truck AssignedTruckFk { get; set; }

        /// <summary>
        /// assigned Trailer
        /// </summary>
        public long? AssignedTrailerId { get; set; }
        [ForeignKey("AssignedTrailerId")]
        public Trailer AssignedTrailersFk { get; set; }


        // todo make sure those are nullable

        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }
        [ForeignKey("TransportTypeId")]
        public TransportType TransportTypeFk { get; set; }


        public virtual int? TransportSubtypeId { get; set; }
        [ForeignKey("TransportSubtypeId")]
        public TransportSubtype TransportSubtypeFk { get; set; }

        public virtual long TrucksTypeId { get; set; }
        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }


        public virtual int? TruckSubtypeId { get; set; }
        [ForeignKey("TruckSubtypeId")]
        public TruckSubtype TruckSubtypeFk { get; set; }


        public virtual int? CapacityId { get; set; }
        [ForeignKey("CapacityId")]
        public Capacity CapacityFk { get; set; }

        #endregion
    }
}