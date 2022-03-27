using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Invoices;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Penalties
{
    [Table("Penalties")]
    public class Penalty : FullAuditedEntity , IMustHaveTenant
    {
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        public PenaltyType Type { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public long? TripId { get; set; }
        public ShippingRequestTrip TripFK { get; set; }
        public long? PointId { get; set; }
        public RoutPoint RoutPointFK { get; set; }
        //shipper
        public long? InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice InvoiceFK { get; set; }
        //carrier
        public long? SubmitInvoiceId {get; set; }
        [ForeignKey(nameof(SubmitInvoiceId))]
        public SubmitInvoice Submitinvoice { get; set; }

        public decimal CommissionValue { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal AmountPreCommestion { get; set; }
        public decimal AmountPostCommestion { get; set; }

        public decimal VatAmount { get; set; }
        public decimal VatPreCommestion { get; set; }
        public decimal VatPostCommestion { get; set; }

        public decimal TotalAmount { get; set; }


    }
}
