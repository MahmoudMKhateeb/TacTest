using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class InvoiceNoteInfoDto
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string Email { get; set; }
        public string Attn { get; set; }
        public string CR { get; set; }
        public string Address { get; set; }
        public string ContractNo { get; set; }
        public string CreationTime { get; set; }
        public NoteType NoteType { get; set; }
        public string ReInvoiceDate { get; set; }
        public string Notes { get; set; }
        public string TenantVatNumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal VatAmount { get; set; }
        public string ReferanceNumber { get; set; }
        public long InvoiceNumber { get; set; }
    }
}
