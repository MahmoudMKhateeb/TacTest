using TACHYON.PricePackages.Dto;

namespace TACHYON.Shipping.DirectRequests.Dto
{
    public class CreateShippingRequestDirectRequestInput
    {
        public long ShippingRequestId { get; set; }
        public int CarrierTenantId { get; set; }

        public PricePackageForPriceCalculationDto PriceCalculationDto { get; set; }
        
        public int? PricePackageId { get; set; }

    }
}