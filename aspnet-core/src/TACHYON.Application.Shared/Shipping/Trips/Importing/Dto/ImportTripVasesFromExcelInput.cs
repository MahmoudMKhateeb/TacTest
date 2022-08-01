using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ImportTripVasesFromExcelInput
    {
        public long ShippingRequestId { get; set; }
        public int? TenantId { get; set; }
        public Guid BinaryObjectId { get; set; }
    }
}
