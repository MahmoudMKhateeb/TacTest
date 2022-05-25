using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class PartialVoidInvoiceDto
    {
        public int TenantId { get; set; }
        public long InvoiceNumber { get; set; }
        public NoteStatus Status { get; set; }
        public List<GetAllInvoiceItemDto> InvoiceItems { get; set; }
    }
}
