using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;

namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequestBids")]
    class ShippingRequestBids :FullAuditedEntity<long>,IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int ShippingRequestId { get; set; }

        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }

        public double price { get; set; }
        public bool IsCancled { get; set; }
        public bool IsAccepted { get; set; }
        public string CancledReason { get; set; }


        public ShippingRequestBids()
        {
            this.IsCancled = false;
            this.IsAccepted = false;

        }

    }
}
