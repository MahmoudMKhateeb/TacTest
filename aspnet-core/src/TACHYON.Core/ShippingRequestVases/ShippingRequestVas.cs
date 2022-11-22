using TACHYON.Vases;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestTripVases;

namespace TACHYON.ShippingRequestVases
{
    [Table("ShippingRequestVases")]
    public class ShippingRequestVas : FullAuditedEntity<long>
    {
        public virtual double? DefualtPrice { get; set; }

        public virtual double? ActualPrice { get; set; }

        public virtual int RequestMaxAmount { get; set; }

        public virtual int RequestMaxCount { get; set; }

        public virtual int VasId { get; set; }

        [ForeignKey("VasId")] public Vas VasFk { get; set; }

        public string OtherVasName { get; set; }

        public virtual long ShippingRequestId { get; set; }

        [ForeignKey("ShippingRequestId")] public ShippingRequest ShippingRequestFk { get; set; }

        //number of trips for each vas, each vas shouldn't assign to more than this number
        public int NumberOfTrips { get; set; }

        public ICollection<ShippingRequestTripVas> ShippingRequestTripVases { get; set; }

        public int? ActorShipperPriceId { get; set; }

        [ForeignKey(nameof(ActorShipperPriceId))]
        public ActorShipperPrice ActorShipperPrice { get; set; }
        
        public int? ActorCarrierPriceId { get; set; }

        [ForeignKey(nameof(ActorCarrierPriceId))]
        public ActorCarrierPrice ActorCarrierPrice { get; set; }
    }
}