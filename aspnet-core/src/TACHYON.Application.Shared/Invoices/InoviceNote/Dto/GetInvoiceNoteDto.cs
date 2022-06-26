using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices.InoviceNote.Dto
{
   public class GetInvoiceNoteDto : EntityDto<long>
    {
        public string ComanyName { get; set; }
        public NoteStatus Status { get; set; }
        public string StatusTitle { get; set; }
        public NoteType NoteType { get; set; }
        public string NoteTypeTitle { get; set; }
        public string WaybillNumber { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Price { get; set; }
        public DateTime GenerationDate { get; set; }
        public decimal TotalValue { get; set; }
        public string VoidType { get; set; }
        public string Remarks { get; set; }
        public long? InvoiceNumber { get; set; }
        public string ReferanceNumber { get; set; }
    }
}
