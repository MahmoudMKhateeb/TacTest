using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DedicatedDynamicInvocies;

namespace TACHYON.DedidcatedDynamicActorInvoices.Dtos
{
    public class CreateOrEditDedicatedActorInvoiceItemDto: EntityDto<long?>
    {
        public long DedicatedShippingRequestTruckId { get; set; }
        public int NumberOfDays { get; set; }
        public decimal PricePerDay { get; set; }
        public int AllNumberDays { get; set; }
        public WorkingDayType WorkingDayType { get; set; }
        public decimal ItemSubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal ItemTotalAmount { get; set; }
        public string Remarks { get; set; }
    }
}
