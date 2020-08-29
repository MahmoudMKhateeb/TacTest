using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Goods.GoodsDetails;
using TACHYON.MultiTenancy;
using TACHYON.Routs;
using TACHYON.Routs.RoutSteps;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequests")]
    public class ShippingRequest : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int RouteId { get; set; }
        public virtual decimal Vas { get; set; }

        public virtual bool IsBid { get; set; }

        public virtual bool IsTachyonDeal { get; set; }

        [ForeignKey("RouteId")]
        public Route RouteFk { get; set; }

        public ICollection<RoutStep> RoutSteps { get; set; }

        public decimal? Price { get; set; }

        public bool? IsPriceAccepted { get; set; }
        public bool? IsRejected { get; set; }

        public long? FatherShippingRequestId { get; set; }

        [ForeignKey("FatherShippingRequestId")]
        public ShippingRequest FatherShippingRequestFk { get; set; }

        public int? CarrierTenantId { get; set; }

        [ForeignKey("CarrierTenantId")]
        public Tenant CarrierTenantFk { get; set; }

        public int NumberOfDrops { get; set; }

        public bool StageOneFinish { get; set; }
        public bool StageTowFinish { get; set; }
        public bool StageThreeFinish { get; set; }


        public virtual long? TrucksTypeId { get; set; }

        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }

        public virtual int? TrailerTypeId { get; set; }

        [ForeignKey("TrailerTypeId")]
        public TrailerType TrailerTypeFk { get; set; }


    }
}