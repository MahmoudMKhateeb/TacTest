using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Reports.ReportDataSources
{
    public class TripDetailsItem
    {
        public string RequestReferenceNumber { get; set; }

        public long? WaybillNumber { get; set; }

        public string TripStatus { get; set; }

        public DateTime? ExpectedPickupDate { get; set; }
        public DateTime? ActualPickupDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        public string CarrierName { get; set; }

        public string ShipperName { get; set;}

        public string OriginCity { get; set; }

        public string DestinationCity { get; set;}

        public string GoodsCategory { get; set; }

        public string DriverName { get; set; }

        public string TruckPlateNumber { get; set; }

        public string TruckType { get; set; }

        public string ShippingType { get; set; }

        public string RequestType { get; set; }

        public int NumberOfDrops { get; set; }

        public IEnumerable<long?> SubWaybills { get; set; }

        public string RouteType { get; set; }

        public string InvoiceStatus { get; set; }

    }
}
