using System;

namespace TACHYON.Dashboards.Carrier.Dto
{
    public class NewDirectRequestListDto
    {

        public long ShippingRequestId { get; set; }
        
        public string ReferenceNumber { get; set; }

        public string CompanyName { get; set; }
        public DateTime CreationTime { get; set; }
        
        
    }
}