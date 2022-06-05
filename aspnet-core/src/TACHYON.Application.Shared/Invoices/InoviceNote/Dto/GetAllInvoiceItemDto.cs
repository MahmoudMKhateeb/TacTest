using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.InoviceNote.Dto
{
    public class GetAllInvoiceItemDto : EntityDto<long?>
    {
        public int TripId { get; set; }
        public long? WaybillNumber { get; set; }
        public int? TripVasId { get; set; }
        public decimal Price { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
