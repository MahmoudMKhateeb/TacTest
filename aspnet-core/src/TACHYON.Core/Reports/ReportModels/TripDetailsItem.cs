namespace TACHYON.Reports.ReportModels
{
    public class TripDetailsItem
    {
        public string RequestReferenceNumber { get; set; }

        public long? WaybillNumber { get; set; }

        public string TripStatus { get; set; }

        public string ExpectedPickupDate { get; set; }
        public string ActualPickupDate { get; set; }

        public string ExpectedDeliveryDate { get; set; }
        public string ActualDeliveryDate { get; set; }

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

        public string SubWaybills { get; set; }

        public string RouteType { get; set; }

        public string InvoiceStatus { get; set; }

    }
}
