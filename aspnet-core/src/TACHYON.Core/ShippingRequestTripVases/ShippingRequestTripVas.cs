using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestVases;

namespace TACHYON.ShippingRequestTripVases
{
    [Table("ShippingRequestTripVases")]
    public class ShippingRequestTripVas : FullAuditedEntity<long>
    {
        public long ShippingRequestVasId { get; set; }

        [ForeignKey("ShippingRequestVasId")]
        public ShippingRequestVas ShippingRequestVasFk { get; set; }
        public int ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")]
        public ShippingRequestTrip ShippingRequestTripFk { get; set; }

        #region Prices
        public decimal? TotalAmount { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }
        public PriceOfferCommissionType? CommissionType { get; set; }
        public decimal? CommissionAmount { get; set; }
        public decimal? CommissionPercentageOrAddValue { get; set; }
        public int Quantity { get; set; } = 1;
        #endregion
    }
}
