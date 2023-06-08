using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.PriceOffers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.ShippingRequestVases;
using TACHYON.Vases;

namespace TACHYON.ShippingRequestTripVases
{
    [Table("ShippingRequestTripVases")]
    public class ShippingRequestTripVas : FullAuditedEntity<long>
    {
        /// <summary>
        /// Nallable bcz of direct trip
        /// </summary>
        public long? ShippingRequestVasId { get; set; }

        [ForeignKey("ShippingRequestVasId")] public ShippingRequestVas ShippingRequestVasFk { get; set; }
        public int ShippingRequestTripId { get; set; }

        [ForeignKey("ShippingRequestTripId")] public ShippingRequestTrip ShippingRequestTripFk { get; set; }
        /// <summary>
        /// This vas is for direct trip
        /// </summary>
        public virtual int? VasId { get; set; }

        [ForeignKey("VasId")] public Vas VasFk { get; set; }
        /// <summary>
        /// This field is for appointment and clearance vas, to identify this vas is for any point
        /// </summary>
        public long? RoutePointId { get; set; }
        [ForeignKey("RoutePointId")]
        public RoutPoint RoutPoint { get; set; }

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