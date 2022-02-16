using TACHYON.PricePackages.Dto.NormalPricePackage;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class CreateShippingRequestDirectRequestInput
    {
        public long ShippingRequestId { get; set; }
        public int CarrierTenantId { get; set; }
        public BidNormalPricePackageDto BidNormalPricePackage { get; set; }

    }
}