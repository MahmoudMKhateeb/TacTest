using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestVases;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ActorCarrierPrices")]
    public class ActorCarrierPrice : FullAuditedEntity
    {
        public long? ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequest { get; set; }


        public long? ShippingRequestVasId { get; set; }
        [ForeignKey(nameof(ShippingRequestVasId))]
        public ShippingRequestVas ShippingRequestVas { get; set; }



        public bool IsActorCarrierHaveInvoice { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TaxVat { get; set; }

    }
}
