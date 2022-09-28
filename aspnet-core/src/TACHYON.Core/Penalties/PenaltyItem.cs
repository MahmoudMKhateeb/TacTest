using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Penalties
{
    [Table("PenaltyItems")]
    public class PenaltyItem : FullAuditedEntity
    {
        public int PenaltyId { get; set; }
        [ForeignKey("PenaltyId")]
        public Penalty PenaltyFk { get; set; }
        public int? ShippingRequestTripId { get; set; }

        [ForeignKey(nameof(ShippingRequestTripId))]
        public ShippingRequestTrip ShippingRequestTripFK { get; set; }

        #region Prices
        public decimal ItemPrice { get; set; }
        public decimal ItemTotalAmountPostVat { get; set; }
        public decimal VatAmount { get; set; }

        #endregion
    }
}
