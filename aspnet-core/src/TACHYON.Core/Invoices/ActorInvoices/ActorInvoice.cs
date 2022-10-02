using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Actors;
using TACHYON.Invoices.Periods;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Invoices.ActorInvoices
{
    [Table("ActorInvoices")]
    public class ActorInvoice : FullAuditedEntity<long>, IMustHaveTenant
    {

        public long? InvoiceNumber { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))] public Tenant Tenant { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public string Note { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public ICollection<ShippingRequestTrip> Trips { get; set; }

        public ActorInvoice()
        {
            Trips = new List<ShippingRequestTrip>();
        }

        public int? ShipperActorId { get; set; }

        [ForeignKey("ShipperActorId")]
        public Actor ShipperActorFk { get; set; }
    }
}
