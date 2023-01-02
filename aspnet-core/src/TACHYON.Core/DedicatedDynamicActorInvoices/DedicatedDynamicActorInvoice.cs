using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.DedicatedDynamicActorInvoices.DedicatedDynamicActorInvoiceItems;
using TACHYON.MultiTenancy;
using Abp.Domain.Entities;
using TACHYON.Actors;
using TACHYON.DedicatedDynamicInvoices;
using Abp.Domain.Entities.Auditing;
using TACHYON.Invoices.ActorInvoices;

namespace TACHYON.DedicatedDynamicActorInvoices
{
    [Table("DedicatedDynamicActorInvoices")]
    public class DedicatedDynamicActorInvoice: FullAuditedEntity<long>, IMustHaveTenant, IDedicatedDynamicInvoiceBase
    {
        /// <summary>
        /// Broker or SAAS tenant that creates internal dedicated dynamic invoice
        /// </summary>
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        public int? ShipperActorId { get; set; }

        [ForeignKey("ShipperActorId")]
        public Actor ShipperActorFk { get; set; }

        public int? CarrierActorId { get; set; }

        [ForeignKey("CarrierActorId")]
        public Actor CarrierActorFk { get; set; }
        public InvoiceAccountType InvoiceAccountType { get; set; }
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequest { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public long? ActorInvoiceId { get; set; }
        [ForeignKey(nameof(ActorInvoiceId))]
        public ActorInvoice ActorInvoice { get; set; }
        public long? ActorSubmitInvoiceId { get; set; }
        [ForeignKey(nameof(ActorSubmitInvoiceId))]
        public ActorSubmitInvoice ActorSubmitInvoice { get; set; }

        public string Notes { get; set; }
        //public InvoiceStatus InvoiceStatus { get; set; }
        public ICollection<DedicatedDynamicActorInvoiceItem> DedicatedDynamicActorInvoiceItems { get; set; }
    }
}
