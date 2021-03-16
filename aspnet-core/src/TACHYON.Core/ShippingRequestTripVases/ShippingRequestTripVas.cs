using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestVases;

namespace TACHYON.ShippingRequestTripVases
{
    [Table("ShippingRequestTripVases")]
    public class ShippingRequestTripVas : FullAuditedEntity<long>
    {
        public long ShippingRequestVasId { get; set; }

        [ForeignKey("ShippingRequestVasId")]
        public ShippingRequestVas ShippingRequestVasFk { get; set; }
        public int ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")]
        public ShippingRequestTrip ShippingRequestTripFk { get; set; }
    }
}
