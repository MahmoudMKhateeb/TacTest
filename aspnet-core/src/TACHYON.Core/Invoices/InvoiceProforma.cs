using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Invoices
{
    [Table("InvoicesProforma")]
    public class InvoiceProforma : CreationAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        [ForeignKey("TenantId")] public Tenant Tenant { get; set; }
        public long RequestId { get; set; }
        [ForeignKey("RequestId")] public ShippingRequest ShippingRequests { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
    }
}