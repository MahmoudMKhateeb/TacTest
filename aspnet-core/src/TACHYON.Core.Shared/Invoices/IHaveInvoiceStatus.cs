namespace TACHYON.Invoices
{
    public interface IHaveInvoiceStatus
    {
        InvoiceStatus Status { set; get; }
    }
}