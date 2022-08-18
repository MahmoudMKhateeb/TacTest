using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
    public class Penalty : FullAuditedEntity , IMayHaveTenant
    {
        public string ReferenceNumber { get; set; }
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        public PenaltyType Type { get; set; }
        public PenaltyStatus Status { get; set; }
        public string SourceFeature { get; set; }
        public int? TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public int? DestinationTenantId { get; set; }
        [ForeignKey(nameof(DestinationTenantId))]
        public Tenant DestinationTenantFK { get; set; }
        //todo should be removed after transfer data to penaltyItem
        public int? ShippingRequestTripId { get; set; }
        [ForeignKey(nameof(ShippingRequestTripId))]

        public ShippingRequestTrip ShippingRequestTripFK { get; set; }
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
        public int? PenaltyComplaintId { get; set; }
        
        [ForeignKey(nameof(PenaltyComplaintId))]
        public PenaltyComplaint PenaltyComplaintFK { get; set; }
        #region prices
        public decimal CommissionValue { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        /// <summary>
        /// == ItemPrice
        /// </summary>
        public decimal AmountPreCommestion { get; set; }
        public decimal AmountPostCommestion { get; set; }
        public decimal TaxVat { get; set; }
        /// <summary>
        /// vat amount (price*taxVat)
        /// </summary>
        public decimal VatAmount { get; set; }
        //public decimal VatPreCommestion { get; set; }
        public decimal VatPostCommestion { get; set; }
        /// <summary>
        /// Total amount with commission + vat with commission
        /// </summary>
        public decimal TotalAmount { get; set; }
        public decimal ItmePrice { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }
        #endregion

        public ICollection<PenaltyItem> PenaltyItems { get; set; }
    }
}
