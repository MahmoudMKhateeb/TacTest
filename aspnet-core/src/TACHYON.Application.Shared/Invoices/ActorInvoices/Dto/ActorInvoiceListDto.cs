using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Invoices.ActorInvoices.Dto
{
    public class ActorInvoiceListDto : CreationAuditedEntityDto<long>
    {
        public long InvoiceNumber { get; set; }
        public int PeriodId { get; set; }
        public string TenantName { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public decimal TotalAmount { get; set; }

        public string ShipperActorName { get; set; }
        public int? ShipperActorId { get; set; }
    }
}