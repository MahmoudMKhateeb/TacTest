using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Goods.GoodsDetails;
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


        public virtual decimal Vas { get; set; }


        public virtual Guid? TrucksTypeId { get; set; }

        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }

        public virtual int? TrailerTypeId { get; set; }

        [ForeignKey("TrailerTypeId")]
        public TrailerType TrailerTypeFk { get; set; }

        public virtual long? GoodsDetailId { get; set; }

        [ForeignKey("GoodsDetailId")]
        public GoodsDetail GoodsDetailFk { get; set; }

        public virtual int? RouteId { get; set; }

        public virtual bool IsBid { get; set; }

        public virtual bool IsTachyonDeal { get; set; }

        [ForeignKey("RouteId")]
        public Route RouteFk { get; set; }

        public ICollection<RoutStep> RoutSteps { get; set; }

    }
}