using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestVases;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ActorShipperPrices")]
    public class ActorShipperPrice :FullAuditedEntity
    {
        public long? ShippingRequestId { get; set; }

        [ForeignKey(nameof(ShippingRequestId))]
        public ShippingRequest ShippingRequest { get; set; }
        
        public long? ShippingRequestVasId { get; set; }
        
        [ForeignKey(nameof(ShippingRequestVasId))]
        public ShippingRequestVas ShippingRequestVas { get; set; }
        public decimal? TotalAmountWithCommission { get; set; }
        public decimal? SubTotalAmountWithCommission { get; set; }
        public decimal? VatAmountWithCommission { get; set; }
        public decimal? TaxVat { get; set; }
    }
}
