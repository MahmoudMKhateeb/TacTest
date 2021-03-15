using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
   public class ShippingRequestTripDriverDocumentDto
    {
        public string DocumentBase64 { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public string ReceiverCode { get; set; }
    }
}
