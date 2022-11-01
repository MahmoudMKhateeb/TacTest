using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.DedicatedDynamicInvocies;
using TACHYON.DedicatedInvoices;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems
{
    [Table("DedicatedDynamicInvoiceItems")]
    public class DedicatedDynamicInvoiceItem :FullAuditedEntity<long>
    {
        public long DedicatedDynamicInvoiceId { get; set; }
        [ForeignKey("DedicatedDynamicInvoiceId")]
        public DedicatedDynamicInvoice DedicatedDynamicInvoice { get; set; }
        public long DedicatedShippingRequestTruckId { get; set; }
        [ForeignKey("DedicatedShippingRequestTruckId")]
        public DedicatedShippingRequestTruck DedicatedShippingRequestTruck { get; set; }
        public int NumberOfDays { get; set; }
        public decimal PricePerDay { get; set; }
        public WorkingDayType WorkingDayType { get; set; }
        public decimal ItemSubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat {get; set; }
        public decimal ItemTotalAmount { get; set; }
        public string Remarks { get; set; }
    }
}
