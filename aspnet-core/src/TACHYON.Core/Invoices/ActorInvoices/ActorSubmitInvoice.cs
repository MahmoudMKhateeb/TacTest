using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Actors;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices.ActorInvoices
{
    [Table("ActorSubmitInvoices")]
    public class ActorSubmitInvoice : FullAuditedEntity<long>, IMustHaveTenant
    {
        public long? ReferencNumber { get; set; }
        public int TenantId { set; get; }
        [ForeignKey(nameof(TenantId))] 
        public Tenant Tenant { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public ICollection<ShippingRequestTrip> Trips { get; set; }
        public ActorSubmitInvoice()
        {
            Trips = new List<ShippingRequestTrip>();
        }
        public int? CarrierActorId { get; set; }

        [ForeignKey("CarrierActorId")]
        public Actor CarrierActorFk { get; set; }
        public SubmitInvoiceStatus Status { get; set; }


    }
}
