using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.ShippingRequestVases
{
    [Table("ShippingRequestTripVases")]
    public class ShippingRequestTripVas : FullAuditedEntity<long>
    {
        public long ShippingRequestVasId { get; set; }
        public ShippingRequestTripVas ShippingRequestVasFk { get; set; }
        public int ShippingRequestTripId { get; set; }
        public ShippingRequestTrip ShippingRequestTripFk { get; set; }
    }
}
