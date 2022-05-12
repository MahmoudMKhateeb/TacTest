namespace TACHYON.Invoices.Dto
{
    public class SubmitInvoiceItemDto
    {
        public decimal? Price { get; set; }
        public string TruckType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string DateWork { get; set; }
        public string Remarks { get; set; }
    }
}