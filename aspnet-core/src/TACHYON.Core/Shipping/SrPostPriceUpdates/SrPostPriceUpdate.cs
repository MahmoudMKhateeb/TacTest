using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    [Table("ShippingRequestPostPriceUpdates")]
    public class SrPostPriceUpdate : FullAuditedEntity<long>
    {
        public long ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequest { get; set; }

        public SrPostPriceUpdateAction Action { get; set; }

        public string RejectionReason { get; set; }

        public string UpdateChanges { get; set; }

        public bool IsApplied { get; set; }

        public long? PriceOfferId { get; set; }

        [ForeignKey(nameof(PriceOfferId))]
        public PriceOffer PriceOffer { get; set; }

        public SrPostPriceUpdateOfferStatus OfferStatus { get; set; }
        
        
    }
}