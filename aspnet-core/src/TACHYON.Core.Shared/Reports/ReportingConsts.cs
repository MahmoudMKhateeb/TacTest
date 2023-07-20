namespace TACHYON.Reports
{
    public static class ReportsNames
    {

        public const string TripDetailsReport = "TripDetailsReport";
        public const string PodPerformanceReport = "PodPerformanceReport";
        public const string FinancialReport = "TripDetailsReport";

    }

    /// <summary>
    /// This class is used to provide default data source in case of there is no data source is a report
    /// </summary>
    public static class ReportDataSourcePaths
    {
        public const string TripDetailsDataSourcePath = "/api/services/app/TripDetailsReportDataSource/GetAll";
        public const string FinancialDataSourcePath = "/api/services/app/TripDetailsReportDataSource/GetAll";
        public const string PodPerformanceDataSourcePath = "/api/services/app/TripDetailsReportDataSource/GetAll";
    }

    public static class ReportParameterNames
    {
        #region Shared Parameters

        public const string ShipperName = "ShipperName";
        public const string CarrierName = "CarrierName";
        public const string TruckType = "TruckType";
        public const string TransportType = "TransportType";
        public const string ShippingType = "ShippingType";
        public const string RequestType = "RequestType";
        public const string RouteType = "RouteType";
        public const string Origin = "Origin";
        public const string Destination = "Destination";
        public const string IsPodSubmitted = "IsPodSubmitted";
        public const string InvoiceIssued = "InvoiceIssued";
        public const string DeliveryDate = "DeliveryDate";
        public const string InvoiceStatus = "InvoiceStatus";
        public const string PodStatus = "PodStatus";
        public const string MinCostWithVat = "MinCostWithVat";
        public const string MinCostWithoutVat = "MinCostWithoutVat";
        public const string MinSellingWithVat = "MinSellingWithVat";
        public const string MinSellingWithoutVat = "MinSellingWithoutVat";
        public const string MinProfitWithVat = "MinProfitWithVat";
        public const string MinProfitWithoutVat = "MinProfitWithoutVat";
        public const string MaxCostWithVat = "MaxCostWithVat";
        public const string MaxCostWithoutVat = "MaxCostWithoutVat";
        public const string MaxSellingWithVat = "MaxSellingWithVat";
        public const string MaxSellingWithoutVat = "MaxSellingWithoutVat";
        public const string MaxProfitWithVat = "MaxProfitWithVat";
        public const string MaxProfitWithoutVat = "MaxProfitWithoutVat";
        

        #endregion
        
    }
}
