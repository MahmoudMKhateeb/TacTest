using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class InvoiceNoteIStatusInputDto
    {
        public int InvoiceNoteId { get; set; }
        public NoteStatus NoteStatus { get; set; }
    }
}
