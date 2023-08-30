namespace TACHYON.Reports.ReportModels
{
    public class PodPerformanceItem
    {
        public string ShipperName { get; set; }

        public string CarrierName { get; set; }

        public string PodStatus { get; set; }

        public string LoadingDate { get; set; }

        public string OffloadingDate { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public long? WaybillNumber { get; set; }

        public string RequestReferenceNumber { get; set; }

        public string ContainerNumber { get; set; }

        public string BookingNumber { get; set; }

        public string ShipperNumber { get; set; }

        public string PlateNumber { get; set; }

        public string DriverName { get; set; }

        public string ShippingType { get; set; }

        public string IsUploadedWithin7Days { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceStatus { get; set; }

        public string AccountManager { get; set; }
    }
}