using TACHYON.Dto;

namespace TACHYON.PricePackages.Dto.TmsPricePackages
{
    public class GetMatchingPricePackagesInput: PagedInputDto
    {
        public long ShippingRequestId { get; set; }
    }
}