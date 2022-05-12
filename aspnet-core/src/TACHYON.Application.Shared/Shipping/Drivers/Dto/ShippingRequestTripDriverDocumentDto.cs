using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverDocumentDto
    {
        public IFormFile Document { get; set; }

        public string ReceiverCode { get; set; }
    }
}