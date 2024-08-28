using Abp.Domain.Entities.Auditing;
using Newtonsoft.Json;
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
        [JsonIgnore]
        public ShippingRequest ShippingRequest { get; set; }


        public long? ShippingRequestVasId { get; set; }
        [ForeignKey(nameof(ShippingRequestVasId))]
        [JsonIgnore]
        public ShippingRequestVas ShippingRequestVas { get; set; }



        public bool IsActorCarrierHaveInvoice { get; set; }
        public decimal? SubTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? TaxVat { get; set; }


          [ForeignKey(nameof(ShippingRequestTrip))]
        public int? ShippingRequestTripId { get; set; }

        [JsonIgnore]
        public ShippingRequestTrip ShippingRequestTrip { get; set; }

    }
}
