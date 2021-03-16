using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Invoices.Groups
{
    [Table("GroupShippingRequests")]
    public class GroupShippingRequests: Entity<long>
    {
        public long RequestId { get; set; }

        [ForeignKey("RequestId")]
        public ShippingRequest ShippingRequests { get; set; }
        public long GroupId { get; set; }

        [ForeignKey("GroupId")]
        public GroupPeriod Group { get; set; }
    }
}
