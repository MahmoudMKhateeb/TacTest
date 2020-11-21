using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class GetAllBidsInput
    {
        public bool IsMatchingOnly { get; set; }
        public bool IsMyBidsOnly { get; set; }
        public long? TruckTypeId { get; set; }
        public long? TruckSubTypeId { get; set; }
        public long? TransportType { get; set; }
        public long? TransportSubType { get; set; }
        public int? CapacityId { get; set; }
    }
}
