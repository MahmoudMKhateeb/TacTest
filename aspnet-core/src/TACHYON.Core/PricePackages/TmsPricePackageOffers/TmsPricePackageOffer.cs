using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.PriceOffers;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.DirectRequests;

namespace TACHYON.PricePackages.TmsPricePackageOffers
{
    [Table("TmsPriceOfferPackageOffers")]
    public class TmsPricePackageOffer : FullAuditedEntity<long>
    {
        
        public long? PriceOfferId { get; set; }

        [ForeignKey(nameof(PriceOfferId))]
        public PriceOffer PriceOffer { get; set; }

        public long? DirectRequestId { get; set; }

        [ForeignKey(nameof(DirectRequestId))]
        public ShippingRequestDirectRequest DirectRequest { get; set; }
        
        public int? TmsPricePackageId { get; set; }
        [ForeignKey(nameof(TmsPricePackageId))]
        public TmsPricePackage TmsPricePackage { get; set; }

        public int? NormalPricePackageId { get; set; }
        [ForeignKey(nameof(NormalPricePackageId))]
        public NormalPricePackage NormalPricePackage { get; set; }
    }
}