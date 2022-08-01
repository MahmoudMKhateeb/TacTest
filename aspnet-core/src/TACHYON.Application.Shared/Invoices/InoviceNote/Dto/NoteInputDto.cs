using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
   public class NoteInputDto : EntityDto<long>
    {
        public string Note { get; set; }
        public bool CanBePrinted { get; set; }
    }
}
