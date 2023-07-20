namespace TACHYON.Reports.ReportModels
{
    public class FinancialItem
    {
        public string RequestId { get; set; }

        public string RequestCreationDate { get; set; }

        public string WaybillNumber { get; set; }

        public string TripCreationDate { get; set; }

        public string TripLoadingDate { get; set; }

        public string TripOffloadingDate { get; set; }

        public string ShipperName { get; set; }

        public string CarrierName { get; set; }

        public long? ShipperInvoiceNumber { get; set; }

        public string ShipperInvoiceStatus { get; set; }
        
        public long? CarrierInvoiceNumber { get; set; }

        public string CarrierInvoiceStatus { get; set; }

        public string PodStatus { get; set; }

        public decimal? CostWithVat { get; set; }

        public decimal? CostWithoutVat { get; set; }

        public decimal? SellingWithVat { get; set; }

        public decimal? SellingWithoutVat { get; set; }

        public decimal? TachyonCommissionWithVat { get; set; }

        public decimal? TachyonCommissionWithoutVat { get; set; }

        public string ShipperInvoiceIssuanceDate { get; set; }

        public string ShipperInvoiceConfirmationDate { get; set; }

        public string TripStatus { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public string TransportType { get; set; }

        public string TruckType { get; set; }
    }
}