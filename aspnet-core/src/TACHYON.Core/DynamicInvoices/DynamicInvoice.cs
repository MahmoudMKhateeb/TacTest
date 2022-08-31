using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.DynamicInvoices
{
    [Table("DynamicInvoices")]
    public class DynamicInvoice : FullAuditedEntity<long>
    {
        /// <summary>
        /// Credit Tenant it's a tenant that we will withdraw from its account balance
        /// <remarks>Usually `CreditTenantId` represent TenantId of a shipper</remarks>
        /// </summary>
        public int? CreditTenantId { get; set; }

        [ForeignKey(nameof(CreditTenantId))]
        public Tenant CreditTenant { get; set; }

        /// <summary>
        /// Debit Tenant it's a tenant that We will make a deposit into his account
        /// <remarks>Usually `DebitTenantId` represent TenantId of a carrier</remarks>
        /// </summary>
        public int? DebitTenantId { get; set; }

        [ForeignKey(nameof(DebitTenantId))]
        public Tenant DebitTenant { get; set; }
        public int TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public ShippingRequestTrip Trip { get; set; }

       
        public List<DynamicInvoiceItem> Items { get; set; }
    }
}