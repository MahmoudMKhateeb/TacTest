using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBidStatuses
{
    [Table("ShippingRequestBidStatuses")]
    public class ShippingRequestBidStatus : FullAuditedEntity
    {
        [Required] public virtual string DisplayName { get; set; }
    }
}