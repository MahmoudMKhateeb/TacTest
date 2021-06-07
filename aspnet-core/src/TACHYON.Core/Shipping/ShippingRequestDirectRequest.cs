using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping
{
    [Table("ShippingRequestDirectRequests")]

    public class ShippingRequestDirectRequest: FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }

        public int CarrierTenantId { get; set; }

        [ForeignKey(nameof(CarrierTenantId))]

        public Tenant Carrier { get; set; }

        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequestFK { get; set; }

        public ShippingRequestDirectRequestStatus Status { get; set; }

        public string RejetcReason { get; set; }
    }
}
