using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.PriceOffers;
using TACHYON.Shipping.DirectRequests;

namespace TACHYON.PricePackages.PricePackageOffers
{
    [Table("PricePackageOffers")]
    public class PricePackageOffer : FullAuditedEntity<long>
    {
        
        public long? PriceOfferId { get; set; }

        [ForeignKey(nameof(PriceOfferId))]
        public PriceOffer PriceOffer { get; set; }

        public long? DirectRequestId { get; set; }

        [ForeignKey(nameof(DirectRequestId))]
        public ShippingRequestDirectRequest DirectRequest { get; set; }
        
        public long? PricePackageId { get; set; }
        [ForeignKey(nameof(PricePackageId))]
        
        public PricePackage PricePackage { get; set; }
    }
}