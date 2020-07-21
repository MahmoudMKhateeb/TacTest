using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Routs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Shipping.ShippingRequests
{
	[Table("ShippingRequests")]
    public class ShippingRequest : FullAuditedEntity<long> , IMustHaveTenant
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
		
        [ForeignKey("RouteId")]
		public Route RouteFk { get; set; }
		
    }
}