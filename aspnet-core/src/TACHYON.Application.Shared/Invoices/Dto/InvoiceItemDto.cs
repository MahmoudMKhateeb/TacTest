namespace TACHYON.Invoices.Dto
{
    public class InvoiceItemDto
    {
        public string Sequence { get; set; }
        public string WayBillNumber { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string TruckType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string DateWork { get; set; }
        public string Remarks { get; set; }
        public string RoundTrip { get; set; }
        public string ContainerNumber { get; set; }



    }
}