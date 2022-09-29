using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices.SubmitInvoices;

namespace TACHYON.Invoices.ActorInvoices.Dto
{
    public class ActorSubmitInvoiceListDto: CreationAuditedEntityDto<long>
    {
        public long ReferencNumber { get; set; }
       // public int PeriodId { get; set; }
        public string TenantName { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }

        public string CarrierActorName { get; set; }
        public int? CarrierActorId { get; set; }
        public string Status { get; set; }
    }
}
