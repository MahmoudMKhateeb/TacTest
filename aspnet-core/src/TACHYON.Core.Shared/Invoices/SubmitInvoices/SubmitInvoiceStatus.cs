using System.ComponentModel;

namespace TACHYON.Invoices.SubmitInvoices
{
    public enum SubmitInvoiceStatus : byte
    {
        New,
        [Description("Claimed")]
        Claim,
        Accepted,
        Rejected,
        Paid,
        UnPaid
    }
}