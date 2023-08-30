using System;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class NewPriceOfferListDto
    {
        public long ShippingRequestId { get; set; }
        public string ReferenceNumber { get; set; }

        public string CompanyName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}