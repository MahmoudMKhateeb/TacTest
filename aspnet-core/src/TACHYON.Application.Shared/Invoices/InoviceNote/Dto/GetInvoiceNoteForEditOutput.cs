using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class GetInvoiceNoteForEditOutput:EntityDto<long>
    {
        public int TenantId { get; set; }
        public NoteType NoteType { get; set; }
        public NoteStatus Status { get; set; }
        public string Remarks { get; set; }
        public string WaybillNumber { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }
        public List<int> InvoiceItems { get; set; }
    }
}
