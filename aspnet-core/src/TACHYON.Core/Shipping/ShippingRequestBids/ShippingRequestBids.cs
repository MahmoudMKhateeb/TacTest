using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.ShippingRequestBids
{
    [Table("ShippingRequestBids")]
    public class ShippingRequestBid : FullAuditedEntity<long>, IMustHaveTenant, IHasIsCanceled
    {
        public int TenantId { get; set; }

        [ForeignKey("TenantId")] public Tenant Tenant { get; set; }

        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")] public ShippingRequest ShippingRequestFk { get; set; }
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Price After commission and vat
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// Total Commission after sum ActualPercentCommission + ActualCommissionValue
        /// </summary>
        public decimal TotalCommission { get; set; }

        /// <summary>
        /// Actual commission percent that used
        /// </summary>
        public decimal ActualPercentCommission { get; set; }

        /// <summary>
        /// Actual Commission value that used
        /// </summary>
        public decimal ActualCommissionValue { get; set; }

        /// <summary>
        /// Actual minimum value that used
        /// </summary>
        public decimal ActualMinCommissionValue { get; set; }

        /// <summary>
        /// price after add total commission
        /// </summary>
        public decimal PriceSubTotal { get; set; }

        /// <summary>
        /// Amount of Tax vat after calculate PriceSubTotal*vatStettings/100
        /// </summary>
        public decimal VatAmount { get; set; }

        public bool IsCancled { get; set; }
        public DateTime? CanceledDate { get; set; }
        public string CancledReason { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public DateTime? AcceptedDate { get; set; }


        public ShippingRequestBid()
        {
            this.IsCancled = false;
            this.IsAccepted = false;
            this.IsRejected = false;
        }
    }
}