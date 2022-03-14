using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ImportTripVasesDto
    {
        public string TripReference { get; set; }
        public long ShippingRequestId { get; set; }
        public int ShippingRequestTripId { get; set; }
        public string VasName { get; set; }
        public long ShippingRequestVasId { get; set; }
        public string Exception { get; set; }
    }
}
