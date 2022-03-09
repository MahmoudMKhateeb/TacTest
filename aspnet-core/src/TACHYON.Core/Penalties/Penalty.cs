using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Penalties
{
    [Table("Penalties")]
    public class Penalty : FullAuditedEntity , IMustHaveTenant
    {
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        public decimal Amount { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public long? ShippingRequestId { get; set; }
        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequest { get; set; }
        public int? ShippingRequestTripId { get; set; }
        [ForeignKey(nameof(ShippingRequestTripId))]
        public ShippingRequestTrip ShippingRequestTrip { get; set; }
    }
}
