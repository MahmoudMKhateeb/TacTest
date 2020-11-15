using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.ShippingRequestBids
{
    [Table("ShippingRequestBids")]
    public class ShippingRequestBid :FullAuditedEntity<long>,IMustHaveTenant,IHasIsCanceled
    {
        public int TenantId { get; set; }
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }
        public double price { get; set; }
        public bool IsCancled { get; set; }
        public DateTime? CanceledDate { get; set; }
        public string CancledReason { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime? AcceptedDate { get; set; }


        public ShippingRequestBid()
        {
            this.IsCancled = false;
            this.IsAccepted = false;
            this.IsRejected = false;
        }

    }
}
