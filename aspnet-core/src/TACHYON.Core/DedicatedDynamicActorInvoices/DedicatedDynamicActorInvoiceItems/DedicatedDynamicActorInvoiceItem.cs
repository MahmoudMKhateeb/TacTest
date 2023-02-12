using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.DedicatedDynamicInvocies;
using TACHYON.DedicatedInvoices;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.DedicatedDynamicActorInvoices.DedicatedDynamicActorInvoiceItems
{
    [Table("DedicatedDynamicActorInvoiceItems")]
    public class DedicatedDynamicActorInvoiceItem: FullAuditedEntity<long>
    {
        public long DedicatedDynamicActorInvoiceId { get; set; }
        [ForeignKey("DedicatedDynamicActorInvoiceId")]
        public DedicatedDynamicActorInvoice DedicatedDynamicActorInvoice { get; set; }
        public long DedicatedShippingRequestTruckId { get; set; }
        [ForeignKey("DedicatedShippingRequestTruckId")]
        public DedicatedShippingRequestTruck DedicatedShippingRequestTruck { get; set; }
        /// <summary>
        /// Present or overtime days that truck attend
        /// </summary>
        public int NumberOfDays { get; set; }
        public decimal PricePerDay { get; set; }
        /// <summary>
        /// All days that truck is rented in
        /// </summary>
        public int AllNumberDays { get; set; }
        public WorkingDayType WorkingDayType { get; set; }
        public decimal ItemSubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal ItemTotalAmount { get; set; }
        public string Remarks { get; set; }
    }
}
