using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class CreateOrEditInvoiceNoteDto : EntityDto<long?>
    {
        public int TenantId { get; set; }
        public NoteType NoteType { get; set; }
        public string Remarks { get; set; }
        public long? InvoiceNumber { get; set; }
        public long? SubmitInvoiceNumber { get; set; }

        public decimal VatAmount { get; set; }
        public string WaybillNumber { get; set; }
        public decimal Price { get; set; }
        public decimal TotalValue { get; set; }
        public bool IsDrafted { get; set; }
        public string InvoiceNoteReferenceNumber { get; set; }
        //public bool IsManual { get; set; }
        public List<GetAllInvoiceItemDto> InvoiceItems { get; set; }
    }
}
