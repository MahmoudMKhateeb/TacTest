using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class GetInvoiceShippingRequestsReportInfoOutput
    {
        public DateTime Date { get;set; } 
        public string WaybillNo { get; set; }
        public string OriginCityName { get; set; }
        public string DestinationCityName { get; set; }
        public string TruckType { get; set; }
        public string Price { get; set; }
        public string Notes { get; set; }
    }
}
