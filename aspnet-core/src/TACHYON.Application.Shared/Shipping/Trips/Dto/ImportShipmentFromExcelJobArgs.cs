using Abp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ImportShipmentFromExcelJobArgs
    {
        public int TenantId { get; set; }

        public Guid BinaryObjectId { get; set; }

        public UserIdentifier User { get; set; }
        public long ShippingRequestId { get; set; }
    }
}
