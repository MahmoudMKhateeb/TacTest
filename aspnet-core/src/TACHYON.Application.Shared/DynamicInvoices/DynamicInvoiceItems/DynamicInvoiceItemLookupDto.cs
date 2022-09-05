using System;

namespace TACHYON.DynamicInvoices.DynamicInvoiceItems
{
    public class DynamicInvoiceItemLookupDto
    {
        public string OriginCityName { get; set; }

        public string DestinationCityName { get; set; }

        public DateTime? WorkDate { get; set; }

        public int? Quantity { get; set; }

        public string PlateNumber { get; set; }

        public string ContainerNumber { get; set; }
    }
}