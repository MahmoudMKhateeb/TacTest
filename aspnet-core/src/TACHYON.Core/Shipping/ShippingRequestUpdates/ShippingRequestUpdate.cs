using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.EntityLogs;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.ShippingRequestUpdates
{
    [Table("ShippingRequestUpdates")]
    public class ShippingRequestUpdate : FullAuditedEntity<Guid>
    {
        // nullable to solve cascade cycle 
        public long? ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequest { get; set; }

        public Guid EntityLogId { get; set; }

        [ForeignKey(nameof(EntityLogId))]
        public EntityLog EntityLog { get; set; }

        public long PriceOfferId { get; set; }

        [ForeignKey(nameof(PriceOfferId))]
        public PriceOffer PriceOffer { get; set; }
        
        /// <summary>
        /// Id of Price Offer Before Repricing
        /// </summary>
        public long? OldPriceOfferId { get; set; }

        [ForeignKey(nameof(OldPriceOfferId))]
        public PriceOffer OldPriceOffer { get; set; }

        public ShippingRequestUpdateStatus Status { get; set; }
    }
}